using System.Windows;

namespace application
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Show the home screen (Window1Control) by default
            ShowHome();
        }

        private void ShowHome()
        {
            var home = new Window1Control();
            home.QuizStartenButton.Click += (s, e) => ShowQuizStart();
            home.KategorienButton.Click += (s, e) => ShowKategorien();
            home.StatistikButton.Click += (s, e) => ShowStatistik();
            MainContent.Content = home;
        }

        private void ShowKategorien()
        {
            var kategorien = new Window2Control();
            // Add handler for "Frage hinzufügen" button if needed
            MainContent.Content = kategorien;
        }

        private void ShowStatistik()
        {
            var statistik = new Window7Control();
            MainContent.Content = statistik;
        }

        private void ShowQuizStart()
        {
            var quiz = new Window4Control();
            quiz.QuizBeginnenButton.Click += (s, e) => ShowQuizLayout();
            quiz.ReturnButton.Click += (s, e) => ShowHome();
            MainContent.Content = quiz;
        }

        private void ShowQuizLayout()
        {
            var quizLayout = new Window5Control();
            MainContent.Content = quizLayout;
        }
    }
}