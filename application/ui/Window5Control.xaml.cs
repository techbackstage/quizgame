using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using QuizGame.Application.Database;
using QuizGame.Application.Model;
using System.Linq;

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
        
        private void AnswerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Disable all buttons to prevent multiple answers
            AnswerButton1.IsEnabled = AnswerButton2.IsEnabled = 
            AnswerButton3.IsEnabled = AnswerButton4.IsEnabled = false;
            
            var button = sender as Button;
            bool isCorrect = button != null && button.Tag != null && (bool)button.Tag;
            
            // Highlight all buttons according to correctness
            HighlightButtons();
            
            // Highlight the clicked button
            if (button != null)
            {
                if (isCorrect)
                {
                    button.Background = System.Windows.Media.Brushes.Green;
                    button.Foreground = System.Windows.Media.Brushes.White;
                }
                else
                {
                    button.Background = System.Windows.Media.Brushes.Red;
                    button.Foreground = System.Windows.Media.Brushes.White;
                }
            }
            
            // Load next question after delay
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, args) => 
            {
                timer.Stop();
                LoadQuestion();
                
                // Re-enable all buttons
                AnswerButton1.IsEnabled = AnswerButton2.IsEnabled = 
                AnswerButton3.IsEnabled = AnswerButton4.IsEnabled = true;
            };
            timer.Start();
        }
        
        private void HighlightButtons()
        {
            // Reset all buttons to default
            ResetButtonStyle(AnswerButton1);
            ResetButtonStyle(AnswerButton2);
            ResetButtonStyle(AnswerButton3);
            ResetButtonStyle(AnswerButton4);
            
            // Highlight correct answers
            if (AnswerButton1.Tag != null && (bool)AnswerButton1.Tag)
                AnswerButton1.Background = System.Windows.Media.Brushes.Green;
                
            if (AnswerButton2.Tag != null && (bool)AnswerButton2.Tag)
                AnswerButton2.Background = System.Windows.Media.Brushes.Green;
                
            if (AnswerButton3.Tag != null && (bool)AnswerButton3.Tag)
                AnswerButton3.Background = System.Windows.Media.Brushes.Green;
                
            if (AnswerButton4.Tag != null && (bool)AnswerButton4.Tag)
                AnswerButton4.Background = System.Windows.Media.Brushes.Green;
        }
        
        private void ResetButtonStyle(Button button)
        {
            button.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#23232b");
            button.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#bfc1ce");
        }
    }
}
