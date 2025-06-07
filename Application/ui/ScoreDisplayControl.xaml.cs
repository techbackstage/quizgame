using System;
using System.Windows;
using System.Windows.Controls;
using QuizGame.Application.Model;

namespace QuizGame.Application.UI
{
    public partial class ScoreDisplayControl : UserControl
    {
        private QuizSession _session;

        public ScoreDisplayControl(QuizSession session)
        {
            InitializeComponent();
            _session = session;
            LoadScore();
        }

        private void LoadScore()
        {
            if (_session != null)
            {
                ScoreTextBlock.Text = $"Deine Punktzahl: {_session.Score} / {_session.TotalQuestions}";
                DateTextBlock.Text = $"Datum: {_session.Date:dd.MM.yyyy HH:mm}";
            }
            else
            {
                ScoreTextBlock.Text = "Keine Sitzungsdaten gefunden.";
                DateTextBlock.Text = string.Empty;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.ShowHome(); 
            }
        }
    }
} 