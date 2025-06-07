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
using QuizGame.Application.Common;
using QuizGame.Application.Services;

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

        private void DeleteCategory(object? parameter)
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

        private void ShowQuestions(object? parameter)
        {
            if (parameter is Category cat)
            {
                var questionsWindow = new QuestionsForCategoryWindow(cat.CategoryId);
                questionsWindow.ShowDialog();
                LoadCategories();
            }
        }
        
        private async void GenerateQuestions(object? parameter)
        {
            if (parameter is Category category)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    var questions = await Task.Run(() => ApiController.Run(category.Name));

                    if (questions.Any())
                    {
                        using (var db = QuizDbContext.getContext())
                        {
                            foreach (var question in questions)
                            {
                                question.CategoryId = category.CategoryId;
                                db.Questions.Add(question);
                            }
                            await db.SaveChangesAsync();
                        }

                        MessageBox.Show($"{questions.Count} Fragen für '{category.Name}' erstellt.", "Fragen generiert",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        LoadCategories(); // Refresh UI
                    }
                    else
                    {
                        MessageBox.Show("Keine Fragen generiert. API möglicherweise nicht verfügbar.",
                            "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Generieren von Fragen: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
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
                    var pdfService = new PdfExportService();
                    pdfService.CreatePdf(selectedCategories, saveFileDialog.FileName);
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
    }
}
