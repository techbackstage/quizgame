using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuizGame.Application.Database;
using QuizGame.Application.Model;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using QuizGame.API;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Microsoft.Win32;
using System.IO;

namespace QuizGame.Application.UI
{
    public partial class Window2Control : UserControl
    {
        public ObservableCollection<Category> Categories { get; set; } = new();

        public ICommand DeleteCategoryCommand { get; }
        public ICommand ShowQuestionsCommand { get; }
        public ICommand GenerateQuestionsCommand { get; }

        // Event for back button
        public event EventHandler? BackButtonClicked;

        public Window2Control()
        {
            InitializeComponent();
            DataContext = this;
            DeleteCategoryCommand = new RelayCommand(DeleteCategory);
            ShowQuestionsCommand = new RelayCommand(ShowQuestions);
            GenerateQuestionsCommand = new RelayCommand(GenerateQuestions);
            LoadCategories();
            AddCategoryButton.Click += AddCategoryButton_Click;
            BackButton.Click += BackButton_Click;
            ExportPdfButton.Click += ExportPdfButton_Click;
        }

        private void LoadCategories()
        {
            Categories.Clear();
            using (var db = QuizDbContext.getContext())
            {
                foreach (var cat in db.Categories.Include(c => c.Questions).ToList())
                    Categories.Add(cat);
            }
            CategoryListPanel.ItemsSource = Categories;
        }

        private void DeleteCategory(object parameter)
        {
            if (parameter is Category cat)
            {
                var result = MessageBox.Show($"Kategorie '{cat.Name}' wirklich löschen?", "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = QuizDbContext.getContext())
                    {
                        var dbCat = db.Categories.Include(c => c.Questions).FirstOrDefault(c => c.CategoryId == cat.CategoryId);
                        if (dbCat != null)
                        {
                            db.Questions.RemoveRange(dbCat.Questions);
                            db.Categories.Remove(dbCat);
                            db.SaveChanges();
                        }
                    }
                    LoadCategories();
                }
            }
        }

        private void ShowQuestions(object parameter)
        {
            if (parameter is Category cat)
            {
                var questionsWindow = new QuestionsForCategoryWindow(cat.CategoryId);
                questionsWindow.ShowDialog();
                LoadCategories();
            }
        }
        
        private void GenerateQuestions(object parameter)
        {
            if (parameter is Category category)
            {
                try
                {
                    // Show loading indicator
                    var cursor = Mouse.OverrideCursor;
                    Mouse.OverrideCursor = Cursors.Wait;
                    
                    // Save questions to database
                    using (var db = QuizDbContext.getContext())
                    {
                        var questions = ApiController.Run(category.Name);
                    
                        if (questions.Count > 0)
                        {
                            foreach (var question in questions)
                            {
                                question.CategoryId = category.CategoryId;

                                db.Questions.Add(question);
                                
                                // Update the UI counter by refreshing the list
                                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                                    var dbCategory = db.Categories
                                        .Include(c => c.Questions)
                                        .FirstOrDefault(c => c.CategoryId == category.CategoryId);
                                        
                                    if (dbCategory != null)
                                    {
                                        var uiCategory = Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                                        if (uiCategory != null)
                                        {
                                            uiCategory.Questions.Add(question);
                                        }
                                    }
                                });
                            }
                            
                            db.SaveChanges();
                            
                            // Show success message
                            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                                MessageBox.Show($"{questions.Count} Fragen für '{category.Name}' erstellt.", "Fragen generiert", 
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                            });
                            
                            // Reload categories to refresh the UI
                            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                                LoadCategories();
                            });
                        }
                        else
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                                MessageBox.Show("Keine Fragen generiert. Möglicherweise ist die API nicht verfügbar oder die Antwort konnte nicht geparst werden. Bitte versuchen Sie es erneut.", 
                                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                            });
                        }
                    }
                    
                    // Restore cursor
                    Mouse.OverrideCursor = cursor;
                }
                catch (System.Exception ex)
                {
                    // Restore cursor on error
                    Mouse.OverrideCursor = null;
                    MessageBox.Show($"Fehler beim Generieren von Fragen: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var addCat = new Window3Control();
            var win = new Window { Content = addCat, Width = 500, Height = 350, WindowStartupLocation = WindowStartupLocation.CenterScreen, Title = "Kategorie hinzufügen" };
            win.ShowDialog();
            LoadCategories();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        // Event handler for SelectAllCheckBox
        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var category in Categories)
            {
                category.IsSelectedForExport = true;
            }
            // Refresh the ItemsControl binding if necessary, though TwoWay binding should handle it.
             CategoryListPanel.Items.Refresh();
        }

        // Event handler for SelectAllCheckBox
        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var category in Categories)
            {
                category.IsSelectedForExport = false;
            }
            // Refresh the ItemsControl binding
            CategoryListPanel.Items.Refresh();
        }
        
        private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategories = Categories.Where(c => c.IsSelectedForExport).ToList();
            if (!selectedCategories.Any())
            {
                MessageBox.Show("Bitte wählen Sie mindestens eine Kategorie für den Export aus.", "Keine Auswahl", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF-Datei (*.pdf)|*.pdf",
                Title = "Kategorien als PDF exportieren",
                FileName = $"Kategorien_Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    CreatePdf(selectedCategories, saveFileDialog.FileName);
                    MessageBox.Show($"Ausgewählte Kategorien wurden erfolgreich nach '{saveFileDialog.FileName}' exportiert.", "Export erfolgreich", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Exportieren der PDF: {ex.Message}", "Exportfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void CreatePdf(List<Category> categoriesToExport, string filePath)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Quiz-Kategorien Export";
            
            XFont fontTitle = new XFont("Arial", 20, XFontStyleEx.Bold);
            XFont fontCategory = new XFont("Arial", 12, XFontStyleEx.Bold);
            XFont fontQuestion = new XFont("Arial", 10, XFontStyleEx.Regular);
            XFont fontCorrectAnswer = new XFont("Arial", 8, XFontStyleEx.Bold);
            XFont fontAnswer = new XFont("Arial", 10, XFontStyleEx.Regular);

            double yPosition = 40; // Initial Y position for drawing
            const double xMargin = 40;
            const double pageHeight = 842; // A4 page height
            const double lineHeight = 18;
            const double paragraphSpacing = 10;

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            Func<double, double> checkAndCreateNewPage = (currentY) => {
                if (currentY > pageHeight - (2 * xMargin)) // Check if space is left, considering bottom margin
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    return xMargin; // Reset Y to top margin
                }
                return currentY; // No change if no new page
            };

            void drawText(string text, XFont font, XBrush brush, double x, double y)
            {
                gfx.DrawString(text, font, brush, new XPoint(x, y));
            }

            drawText("Exportierte Quiz-Kategorien", fontTitle, XBrushes.Black, xMargin, yPosition);
            yPosition += fontTitle.GetHeight() + paragraphSpacing * 2;

            using (var db = QuizDbContext.getContext())
            {
                foreach (var category in categoriesToExport)
                {
                    yPosition = checkAndCreateNewPage(yPosition);
                    gfx.DrawString($"Kategorie: {category.Name}", fontCategory, XBrushes.DarkBlue, xMargin, yPosition);
                    yPosition += fontCategory.GetHeight() + paragraphSpacing;

                    // Load questions for the category if not already loaded (should be eager-loaded ideally)
                    var dbCategory = db.Categories.Include(c => c.Questions).ThenInclude(q => q.Answers).FirstOrDefault(c => c.CategoryId == category.CategoryId);
                    if (dbCategory == null || !dbCategory.Questions.Any())
                    {
                        yPosition = checkAndCreateNewPage(yPosition);
                        gfx.DrawString("  Keine Fragen in dieser Kategorie.", fontAnswer, XBrushes.Gray, xMargin + 10, yPosition);
                        yPosition += lineHeight;
                        continue;
                    }

                    foreach (var question in dbCategory.Questions)
                    {
                        yPosition = checkAndCreateNewPage(yPosition);
                        gfx.DrawString($"Frage: {question.Text}", fontQuestion, XBrushes.Black, xMargin + 10, yPosition);
                        yPosition += fontQuestion.GetHeight() + paragraphSpacing / 2;

                        if (!question.Answers.Any())
                        {
                            yPosition = checkAndCreateNewPage(yPosition);
                            gfx.DrawString("    Keine Antworten für diese Frage.", fontAnswer, XBrushes.Gray, xMargin + 20, yPosition);
                            yPosition += lineHeight;
                            continue;
                        }
                        
                        foreach (var answer in question.Answers.OrderBy(a => a.AnswerOptionId)) // Ensure consistent order
                        {
                            yPosition = checkAndCreateNewPage(yPosition);
                            XFont currentAnswerFont = answer.IsCorrect ? fontCorrectAnswer : fontAnswer;
                            XBrush currentAnswerBrush = answer.IsCorrect ? XBrushes.Green : XBrushes.Black;
                            string prefix = answer.IsCorrect ? "Richtige Antwort: " : "Antwort: ";
                            
                            gfx.DrawString($"{prefix}{answer.Text}", currentAnswerFont, currentAnswerBrush, xMargin + 20, yPosition);
                            yPosition += currentAnswerFont.GetHeight();
                        }
                        yPosition += paragraphSpacing; // Space after each question block
                    }
                    yPosition += paragraphSpacing * 1.5; // Extra space after each category
                }
            }
            document.Save(filePath);
        }
    }
    // Simple RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly System.Action<object> _execute;
        private readonly System.Predicate<object> _canExecute;
        public RelayCommand(System.Action<object> execute, System.Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
        public event System.EventHandler CanExecuteChanged { add { } remove { } }
    }
}
