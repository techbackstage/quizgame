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
using System.Windows.Threading;

namespace QuizGame.Application.UI
{
    public partial class Window5Control : UserControl
    {
        private readonly Category? _selectedCategory;
        private int _questionCounter = 0;
        private int _currentScore = 0;
        private const int TotalQuestions = 10;
        private const int TimePerQuestion = 20; // seconds
        private Question? _currentQuestion;

        private QuizSession _currentQuizSession = new QuizSession { Date = DateTime.Now };
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly DispatcherTimer _questionTimer = new DispatcherTimer();
        private int _remainingTime;
        private bool _canAnswer;

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
            AnswerCommand = new RelayCommand(SubmitAnswer, (p) => _canAnswer);
            AnswersItemsControl.ItemsSource = CurrentAnswers;
            
            _questionTimer.Interval = TimeSpan.FromSeconds(1);
            _questionTimer.Tick += QuestionTimer_Tick;

            _stopwatch.Start();
            LoadQuestion();
        }

        private void QuestionTimer_Tick(object? sender, EventArgs e)
        {
            _remainingTime--;
            TimerTextBlock.Text = $"00:{_remainingTime:00}";
            TimeProgressBar.Value = (_remainingTime / (double)TimePerQuestion) * 100;
            if (_remainingTime <= 0)
            {
                SubmitAnswer(null); // Timeout
            }
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
                
                ExplanationPanel.Visibility = Visibility.Collapsed;
                AnswersItemsControl.IsEnabled = true;
                _canAnswer = true;
                CommandManager.InvalidateRequerySuggested();

                QuestionProgressTextBlock.Text = $"Frage {_questionCounter} von {TotalQuestions}";
                ScoreTextBlock.Text = $"Punkte: {_currentScore}";

                using (var db = QuizDbContext.GetContext())
                {
                    var questionQuery = db.Questions.Include(q => q.Answers).AsQueryable();
                    if (_selectedCategory != null)
                        questionQuery = questionQuery.Where(q => q.CategoryId == _selectedCategory.CategoryId);
                    
                    _currentQuestion = questionQuery.OrderBy(q => Guid.NewGuid()).FirstOrDefault();

                    if (_currentQuestion != null)
                    {
                        QuestionTextBlock.Text = _currentQuestion.Text;
                        CurrentAnswers.Clear();
                        var answers = _currentQuestion.Answers.OrderBy(a => Guid.NewGuid()).ToList();
                        foreach (var answer in answers)
                            CurrentAnswers.Add(new AnswerViewModel(answer));
                        
                        _remainingTime = TimePerQuestion;
                        TimerTextBlock.Text = $"00:{_remainingTime:00}";
                        TimeProgressBar.Value = 100;
                        _questionTimer.Start();
                    }
                    else
                    {
                        QuestionTextBlock.Text = "Keine Frage für diese Kategorie gefunden.";
                        CurrentAnswers.Clear();
                        _questionTimer.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Frage: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SubmitAnswer(object? parameter)
        {
            _questionTimer.Stop();
            _canAnswer = false;
            CommandManager.InvalidateRequerySuggested();
            AnswersItemsControl.IsEnabled = false;

            if (parameter is AnswerViewModel selectedAnswer)
            {
                selectedAnswer.IsSelected = true;
                if (selectedAnswer.IsCorrect)
                {
                    _currentScore++;
                    ScoreTextBlock.Text = $"Punkte: {_currentScore}";
                }
            }

            foreach (var answer in CurrentAnswers)
            {
                answer.IsRevealed = true;
            }
            
            ExplanationTextBlock.Text = string.IsNullOrWhiteSpace(_currentQuestion?.Explanation)
                ? "Für diese Frage ist keine Erklärung verfügbar."
                : _currentQuestion.Explanation;
            ExplanationPanel.Visibility = Visibility.Visible;
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            LoadQuestion();
        }

        private void EndQuiz()
        {
            _stopwatch.Stop();
            _currentQuizSession.Score = _currentScore;
            _currentQuizSession.TotalQuestions = TotalQuestions;
            _currentQuizSession.CompletionTime = _stopwatch.Elapsed;
            _currentQuizSession.CategoryId = _selectedCategory?.CategoryId;

            using (var db = QuizDbContext.GetContext())
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
