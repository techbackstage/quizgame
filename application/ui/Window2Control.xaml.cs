using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuizGame.Application.Database;
using QuizGame.Application.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using QuizGame.API;

namespace QuizGame.Application.UI
{
    public partial class Window2Control : UserControl
    {
        public ObservableCollection<Category> Categories { get; set; } = new();

        public ICommand DeleteCategoryCommand { get; }
        public ICommand ShowQuestionsCommand { get; }
        public ICommand GenerateQuestionsCommand { get; }

        public Window2Control()
        {
            InitializeComponent();
            DataContext = this;
            DeleteCategoryCommand = new RelayCommand(DeleteCategory);
            ShowQuestionsCommand = new RelayCommand(ShowQuestions);
            GenerateQuestionsCommand = new RelayCommand(GenerateQuestions);
            LoadCategories();
            AddCategoryButton.Click += AddCategoryButton_Click;
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
        
        private async void GenerateQuestions(object parameter)
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
                        var questions = ApiController.Run("Tennis");
                        
                        if (questions.Count > 0)
                        {
                            foreach (var question in questions)
                            {
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
                            
                            db.SaveChangesAsync();
                            
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
                                MessageBox.Show("Keine Fragen generiert. Bitte versuchen Sie es erneut.", 
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
            var win = new Window { Content = addCat, Width = 400, Height = 250, WindowStartupLocation = WindowStartupLocation.CenterScreen, Title = "Kategorie hinzufügen" };
            win.ShowDialog();
            LoadCategories();
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
