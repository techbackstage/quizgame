using System;
using System.Windows;
using QuizGame.Application.Database;
using QuizGame.Application.Model;
using QuizGame.Application.UI;

namespace QuizGame.Application
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                // Ensure the database is created
                using (var db = new QuizDbContext())
                {
                    db.Database.EnsureCreated();
                }
                InitializeComponent();
                // Show the home screen (Window1Control) by default
                ShowHome();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing main window: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            quiz.QuizBeginnenButton.Click += (s, e) => ShowQuizLayout(quiz.SelectedCategory);
            quiz.ReturnButton.Click += (s, e) => ShowHome();
            MainContent.Content = quiz;
        }

        private void ShowQuizLayout(Category selectedCategory)
        {
            var quizLayout = new Window5Control(selectedCategory);
            MainContent.Content = quizLayout;
        }
    }
}