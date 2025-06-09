using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuizGame.Application.Database;
using QuizGame.Application.Model;
using Microsoft.EntityFrameworkCore; // Füge diese Zeile hinzu
using QuizGame.Application.Common;

namespace QuizGame.Application.UI
{
    public partial class QuestionsForCategoryWindow : Window
    {
        public ObservableCollection<Question> Questions { get; set; } = new();
        public ICommand DeleteQuestionCommand { get; }
        public ICommand EditQuestionCommand { get; }
        private int _categoryId;

        public QuestionsForCategoryWindow(int categoryId)
        {
            InitializeComponent();
            DataContext = this;
            _categoryId = categoryId;
            DeleteQuestionCommand = new RelayCommand(DeleteQuestion);
            EditQuestionCommand = new RelayCommand(EditQuestion);
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            Questions.Clear();
            using (var db = QuizDbContext.GetContext())
            {
                var cat = db.Categories.FirstOrDefault(c => c.CategoryId == _categoryId);
                CategoryTitle.Text = cat != null ? $"Fragen für Kategorie: {cat.Name}" : "Fragen";
                foreach (var q in db.Questions.Where(q => q.CategoryId == _categoryId).ToList())
                    Questions.Add(q);
            }
            QuestionsList.ItemsSource = Questions;
        }

        private void DeleteQuestion(object? parameter)
        {
            if (parameter is Question q)
            {
                var result = MessageBox.Show($"Frage wirklich löschen?", "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = QuizDbContext.GetContext())
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

        private void EditQuestion(object? parameter)
        {
            if (parameter is Question question)
            {
                var editWindow = new EditQuestionWindow(question);
                if (editWindow.ShowDialog() == true)
                {
                    using (var db = QuizDbContext.GetContext())
                    {
                        var dbQuestion = db.Questions
                            .Include(q => q.Answers)
                            .FirstOrDefault(q => q.QuestionId == question.QuestionId);
                        
                        if (dbQuestion != null)
                        {
                            // Aktualisiere die Frage
                            dbQuestion.Text = editWindow.QuestionText;
                            dbQuestion.Explanation = editWindow.Explanation;
                            
                            // Aktualisiere Antworten
                            // HINWEIS: Dies aktualisiert nur bestehende Antworten. Das Hinzufügen/Entfernen von Antworten
                            // im Editierfenster wird derzeit nicht unterstützt.
                            for (int i = 0; i < editWindow.Answers.Count; i++)
                            {
                                if (i < dbQuestion.Answers.Count)
                                {
                                    dbQuestion.Answers[i].Text = editWindow.Answers[i].Text;
                                    dbQuestion.Answers[i].IsCorrect = editWindow.Answers[i].IsCorrect;
                                }
                            }
                            
                            db.SaveChanges();
                            LoadQuestions();
                        }
                    }
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
