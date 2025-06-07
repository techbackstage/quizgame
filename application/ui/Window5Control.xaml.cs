using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using QuizGame.Application.Database;
using QuizGame.Application.Model;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Input;
using QuizGame.Application.ViewModels;
using QuizGame.Application.Common;

namespace QuizGame.Application.UI
{
    public partial class Window5Control : UserControl
    {
        private readonly Category? _selectedCategory;
        private int _questionCounter = 0;
        private int _currentScore = 0;
        private const int TotalQuestions = 10;
        private QuizSession _currentQuizSession = new QuizSession { Date = DateTime.Now };
        private readonly Stopwatch _stopwatch = new Stopwatch();
        
        public ObservableCollection<AnswerViewModel> CurrentAnswers { get; set; }
        public ICommand AnswerCommand { get; }

        public Window5Control() : this(null)
        {
            // Common initialization can go here if needed, or leave empty if all is in the other constructor
        }
        
        public Window5Control(Category? selectedCategory)
        {
            InitializeComponent();
            DataContext = this;
            _selectedCategory = selectedCategory;

            CurrentAnswers = new ObservableCollection<AnswerViewModel>();
            AnswerCommand = new RelayCommand(SubmitAnswer);
            AnswersItemsControl.ItemsSource = CurrentAnswers;
            
            _stopwatch.Start();
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            try
            {
                _questionCounter++;
                if (_questionCounter > TotalQuestions)
                {
                    EndQuiz();
                    return;
                }
                
                QuestionProgressTextBlock.Text = $"Frage {_questionCounter} von {TotalQuestions}";
                
                using (var db = QuizDbContext.getContext())
                {
                    var questionQuery = db.Questions.Include(q => q.Answers).AsQueryable();
                    if (_selectedCategory != null)
                    {
                        questionQuery = questionQuery.Where(q => q.CategoryId == _selectedCategory.CategoryId);
                    }
                    
                    var question = questionQuery.OrderBy(q => Guid.NewGuid()).FirstOrDefault();

                    if (question != null)
                    {
                        QuestionTextBlock.Text = question.Text;
                        CurrentAnswers.Clear();
                        var answers = question.Answers.OrderBy(a => Guid.NewGuid()).ToList();
                        foreach (var answer in answers)
                        {
                            CurrentAnswers.Add(new AnswerViewModel(answer));
                        }
                    }
                    else
                    {
                        QuestionTextBlock.Text = "Keine Frage für diese Kategorie gefunden.";
                        CurrentAnswers.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Frage: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async void SubmitAnswer(object? parameter)
        {
            if (parameter is AnswerViewModel selectedAnswer)
            {
                selectedAnswer.IsSelected = true;
                if (selectedAnswer.IsCorrect)
                {
                    _currentScore++;
                }

                foreach (var answer in CurrentAnswers)
                {
                    answer.IsRevealed = true;
                }

                await Task.Delay(2000); 
                
                LoadQuestion();
            }
        }
        
        private void EndQuiz()
        {
            _stopwatch.Stop();
            _currentQuizSession.Score = _currentScore;
            _currentQuizSession.TotalQuestions = TotalQuestions;
            _currentQuizSession.CompletionTime = _stopwatch.Elapsed;
            _currentQuizSession.CategoryId = _selectedCategory?.CategoryId;

            using (var db = QuizDbContext.getContext())
            {
                db.QuizSessions.Add(_currentQuizSession);
                db.SaveChanges();
            }

            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.ShowScoreDisplay(_currentQuizSession);
            }
        }
    }
}
