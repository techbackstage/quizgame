using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using application.Data;
using application.Models;
using System.Linq;

namespace application
{
    public partial class Window5Control : UserControl
    {
        public Window5Control()
        {
            InitializeComponent();
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            try
            {
                using (var db = new QuizDbContext())
                {
                    // Try to get a sports question, or add one if not present
                    var question = db.Questions
                        .Include(q => q.AnswerOptions)
                        .Where(q => q.Text.Contains("Fußballmannschaft"))
                        .OrderBy(q => q.QuestionId)
                        .FirstOrDefault();

                    if (question == null)
                    {
                        // Add category if not exists
                        var category = db.Categories.FirstOrDefault(c => c.Name == "Sport");
                        if (category == null)
                        {
                            category = new Category { Name = "Sport", Description = "Sportfragen" };
                            db.Categories.Add(category);
                            db.SaveChanges();
                        }

                        var newQuestion = new Question
                        {
                            Text = "Wie viele Spieler hat eine Fußballmannschaft auf dem Feld?",
                            DifficultyLevel = 1,
                            CategoryId = category.CategoryId,
                            AnswerOptions = new System.Collections.Generic.List<AnswerOption>
                            {
                                new AnswerOption { Text = "9", IsCorrect = false },
                                new AnswerOption { Text = "10", IsCorrect = false },
                                new AnswerOption { Text = "11", IsCorrect = true },
                                new AnswerOption { Text = "12", IsCorrect = false }
                            }
                        };
                        db.Questions.Add(newQuestion);
                        db.SaveChanges();

                        // Reload with answer options
                        question = db.Questions
                            .Include(q => q.AnswerOptions)
                            .Where(q => q.Text.Contains("Fußballmannschaft"))
                            .OrderBy(q => q.QuestionId)
                            .FirstOrDefault();
                    }

                    // Defensive: check for nulls
                    if (question != null && question.AnswerOptions != null)
                    {
                        var answers = question.AnswerOptions.OrderBy(a => a.AnswerOptionId).ToList();
                        QuestionTextBlock.Text = question.Text;
                        AnswerButton1.Content = (answers.Count > 0) ? answers[0].Text : "";
                        AnswerButton2.Content = (answers.Count > 1) ? answers[1].Text : "";
                        AnswerButton3.Content = (answers.Count > 2) ? answers[2].Text : "";
                        AnswerButton4.Content = (answers.Count > 3) ? answers[3].Text : "";
                    }
                    else
                    {
                        QuestionTextBlock.Text = "Keine Frage gefunden.";
                        AnswerButton1.Content = AnswerButton2.Content = AnswerButton3.Content = AnswerButton4.Content = "";
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Fehler beim Laden der Frage: {ex.Message}", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
