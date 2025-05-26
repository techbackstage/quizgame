using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using QuizGame.Application.Database;
using QuizGame.Application.Model;
using System.Linq;
using System.Threading.Tasks;

namespace QuizGame.Application.UI
{
    public partial class Window5Control : UserControl
    {
        private readonly Category? _selectedCategory;
        private int _questionCounter = 0;
        private const int TotalQuestions = 10;
        
        public Window5Control()
        {
            InitializeComponent();
            
            // Add event handlers for answer buttons
            AnswerButton1.Click += AnswerButton_Click;
            AnswerButton2.Click += AnswerButton_Click;
            AnswerButton3.Click += AnswerButton_Click;
            AnswerButton4.Click += AnswerButton_Click;
            
            LoadQuestion();
        }
        
        public Window5Control(Category selectedCategory)
        {
            InitializeComponent();
            _selectedCategory = selectedCategory;
            
            // Add event handlers for answer buttons
            AnswerButton1.Click += AnswerButton_Click;
            AnswerButton2.Click += AnswerButton_Click;
            AnswerButton3.Click += AnswerButton_Click;
            AnswerButton4.Click += AnswerButton_Click;
            
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            try
            {
                _questionCounter++;
                if (_questionCounter > TotalQuestions)
                {
                    // End of quiz reached
                    MessageBox.Show("Quiz beendet!", "Ende", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                
                // Reset button colors to default
                ResetAllButtonStyles();
                
                // Update progress text
                QuestionProgressTextBlock.Text = $"Frage {_questionCounter} von {TotalQuestions}";
                
                using (var db = QuizDbContext.getContext())
                {
                    // Get a random question from the selected category if provided
                    var questionQuery = db.Questions.Include(q => q.AnswerOptions).AsQueryable();
                    
                    if (_selectedCategory != null)
                    {
                        questionQuery = questionQuery.Where(q => q.CategoryId == _selectedCategory.CategoryId);
                    }
                    
                    var question = questionQuery
                        .OrderBy(q => Guid.NewGuid()) // Random order
                        .FirstOrDefault();

                    if (question == null)
                    {
                        // If no questions found in the selected category, create a sample question
                        var category = _selectedCategory ?? 
                            db.Categories.FirstOrDefault() ?? 
                            CreateDefaultCategory(db);

                        var newQuestion = new Question
                        {
                            Text = $"Beispielfrage für {category.Name}?",
                            DifficultyLevel = 1,
                            CategoryId = category.CategoryId,
                            AnswerOptions = new System.Collections.Generic.List<AnswerOption>
                            {
                                new AnswerOption { Text = "Antwort 1", IsCorrect = false },
                                new AnswerOption { Text = "Antwort 2", IsCorrect = false },
                                new AnswerOption { Text = "Antwort 3", IsCorrect = true },
                                new AnswerOption { Text = "Antwort 4", IsCorrect = false }
                            }
                        };
                        db.Questions.Add(newQuestion);
                        db.SaveChanges();

                        // Reload with answer options
                        question = db.Questions
                            .Include(q => q.AnswerOptions)
                            .Where(q => q.QuestionId == newQuestion.QuestionId)
                            .FirstOrDefault();
                    }

                    // Defensive: check for nulls
                    if (question != null && question.AnswerOptions != null)
                    {
                        var answers = question.AnswerOptions.OrderBy(a => Guid.NewGuid()).ToList(); // Randomize answers
                        QuestionTextBlock.Text = question.Text;
                        AnswerButton1.Content = (answers.Count > 0) ? answers[0].Text : "";
                        AnswerButton2.Content = (answers.Count > 1) ? answers[1].Text : "";
                        AnswerButton3.Content = (answers.Count > 2) ? answers[2].Text : "";
                        AnswerButton4.Content = (answers.Count > 3) ? answers[3].Text : "";
                        
                        // Store correct answer for validation
                        AnswerButton1.Tag = (answers.Count > 0) ? answers[0].IsCorrect : false;
                        AnswerButton2.Tag = (answers.Count > 1) ? answers[1].IsCorrect : false;
                        AnswerButton3.Tag = (answers.Count > 2) ? answers[2].IsCorrect : false;
                        AnswerButton4.Tag = (answers.Count > 3) ? answers[3].IsCorrect : false;
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
        
        private Category CreateDefaultCategory(QuizDbContext db)
        {
            var category = new Category { Name = "Allgemein", Description = "Allgemeine Fragen" };
            db.Categories.Add(category);
            db.SaveChanges();
            return category;
        }
        
        private async void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            AnswerButton1.IsEnabled = false;
            AnswerButton2.IsEnabled = false;
            AnswerButton3.IsEnabled = false;
            AnswerButton4.IsEnabled = false;

            var correctBrush = Brushes.Green;
            var correctBorderBrush = Brushes.DarkGreen; // Darker green for border
            var correctForegroundBrush = Brushes.White;

            var defaultBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#23232b"));
            var defaultBorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#262631"));
            var defaultForegroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bfc1ce"));

            Button[] answerButtons = { AnswerButton1, AnswerButton2, AnswerButton3, AnswerButton4 };

            foreach (Button btn in answerButtons)
            {
                bool isButtonCorrect = btn.Tag is bool bVal && bVal; // Evaluate for debug and logic
                
                if (isButtonCorrect) // Use the pre-evaluated boolean
                {
                    btn.Background = correctBrush;
                    btn.BorderBrush = correctBorderBrush;
                    btn.Foreground = correctForegroundBrush;
                }
                else
                {
                    btn.Background = defaultBrush;
                    btn.BorderBrush = defaultBorderBrush;
                    btn.Foreground = defaultForegroundBrush;
                }
            }

            await System.Threading.Tasks.Task.Delay(2000);

            LoadQuestion();

            if (_questionCounter <= TotalQuestions)
            {
                AnswerButton1.IsEnabled = true;
                AnswerButton2.IsEnabled = true;
                AnswerButton3.IsEnabled = true;
                AnswerButton4.IsEnabled = true;
                // Reset styles for next question explicitly here if not done in LoadQuestion
                // ResetAllButtonStyles(); // Consider if this is needed here or in LoadQuestion
            }
        }
        
        private void ResetButtonStyle(Button button)
        {
            // Reset only the dynamically changed properties to their default visual state.
            // These values should match the initial state defined in XAML (either in Style or direct attributes).
            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#23232b"));
            button.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bfc1ce"));
            button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#262631"));
            // Do NOT clear the style, set BorderThickness, or call UpdateLayout(), 
            // as these can interfere with the XAML-defined style and layout.
        }
        
        private void ResetAllButtonStyles()
        {
            ResetButtonStyle(AnswerButton1);
            ResetButtonStyle(AnswerButton2);
            ResetButtonStyle(AnswerButton3);
            ResetButtonStyle(AnswerButton4);
        }
    }
}
