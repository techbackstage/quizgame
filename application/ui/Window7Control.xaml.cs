using System.Windows.Controls;
using QuizGame.Application.Database;
using QuizGame.Application.Model;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System;

namespace QuizGame.Application.UI
{
    public partial class Window7Control : UserControl
    {
        public Window7Control()
        {
            InitializeComponent();
            Loaded += Window7Control_Loaded;
        }

        private void Window7Control_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAndDrawGraph();
        }

        private void LoadAndDrawGraph()
        {
            using (var db = QuizDbContext.getContext())
            {
                var sessions = db.QuizSessions.OrderBy(s => s.Date).ToList();
                DrawScoreGraph(sessions);
                CalculateAndDisplayAverage(sessions);
            }
        }

        private void CalculateAndDisplayAverage(List<QuizSession> sessions)
        {
            if (sessions.Any())
            {
                double average = sessions.Average(s => s.Score);
                AverageScoreTextBlock.Text = average.ToString("F1");
            }
            else
            {
                AverageScoreTextBlock.Text = "-";
            }
        }

        private void DrawScoreGraph(List<QuizSession> sessions)
        {
            ScoreHistoryCanvas.Children.Clear();
            if (!sessions.Any()) return;

            double canvasHeight = ScoreHistoryCanvas.ActualHeight;
            if (canvasHeight == 0) canvasHeight = 200;
            double canvasWidth = ScoreHistoryCanvas.ActualWidth;
            if (canvasWidth == 0) canvasWidth = 700;

            double maxScore = 10;
            double barWidth = Math.Max(10, (canvasWidth / sessions.Count) * 0.6);
            double barSpacing = (canvasWidth - (barWidth * sessions.Count)) / (sessions.Count + 1);
            if (barSpacing < 5) barSpacing = 5;
            if (sessions.Count == 1) barWidth = canvasWidth * 0.2;

            double leftOffset = barSpacing;

            SolidColorBrush barBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4e7cff"));
            SolidColorBrush textBrush = Brushes.WhiteSmoke;

            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                double barHeight = (session.Score / maxScore) * (canvasHeight * 0.8);
                if (barHeight < 1) barHeight = 1;

                Rectangle bar = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = barBrush
                };

                Canvas.SetLeft(bar, leftOffset);
                Canvas.SetTop(bar, canvasHeight - barHeight - 20);
                ScoreHistoryCanvas.Children.Add(bar);

                TextBlock dateLabel = new TextBlock
                {
                    Text = session.Date.ToString("dd.MM"),
                    FontSize = 9,
                    Foreground = textBrush,
                    TextAlignment = TextAlignment.Center,
                    Width = barWidth
                };
                Canvas.SetLeft(dateLabel, leftOffset);
                Canvas.SetTop(dateLabel, canvasHeight - 18);
                ScoreHistoryCanvas.Children.Add(dateLabel);

                TextBlock scoreLabel = new TextBlock
                {
                    Text = session.Score.ToString(),
                    FontSize = 10,
                    Foreground = textBrush,
                    TextAlignment = TextAlignment.Center,
                    Width = barWidth
                };
                Canvas.SetLeft(scoreLabel, leftOffset);
                double scoreLabelTop = canvasHeight - barHeight - 20 - 12;
                if (scoreLabelTop < 0) scoreLabelTop = 0;
                Canvas.SetTop(scoreLabel, scoreLabelTop);
                ScoreHistoryCanvas.Children.Add(scoreLabel);

                leftOffset += barWidth + barSpacing;
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
