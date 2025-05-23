using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuizGame.Application.Database;
using QuizGame.Application.Model;

namespace QuizGame.Application.UI
{
    public partial class QuestionsForCategoryWindow : Window
    {
        public ObservableCollection<Question> Questions { get; set; } = new();
        public ICommand DeleteQuestionCommand { get; }
        private int _categoryId;

        public QuestionsForCategoryWindow(int categoryId)
        {
            InitializeComponent();
            DataContext = this;
            _categoryId = categoryId;
            DeleteQuestionCommand = new RelayCommand(DeleteQuestion);
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            Questions.Clear();
            using (var db = QuizDbContext.getContext())
            {
                var cat = db.Categories.FirstOrDefault(c => c.CategoryId == _categoryId);
                CategoryTitle.Text = cat != null ? $"Fragen für Kategorie: {cat.Name}" : "Fragen";
                foreach (var q in db.Questions.Where(q => q.CategoryId == _categoryId).ToList())
                    Questions.Add(q);
            }
            QuestionsList.ItemsSource = Questions;
        }

        private void DeleteQuestion(object parameter)
        {
            if (parameter is Question q)
            {
                var result = MessageBox.Show($"Frage wirklich löschen?", "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = QuizDbContext.getContext())
                    {
                        var dbQ = db.Questions.FirstOrDefault(x => x.QuestionId == q.QuestionId);
                        if (dbQ != null)
                        {
                            db.Questions.Remove(dbQ);
                            db.SaveChanges();
                        }
                    }
                    LoadQuestions();
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
