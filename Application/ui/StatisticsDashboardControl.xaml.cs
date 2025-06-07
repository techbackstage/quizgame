using Microsoft.EntityFrameworkCore;
using QuizGame.Application.Database;
using QuizGame.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace QuizGame.Application.UI
{
    public partial class StatisticsDashboardControl : UserControl
    {
        public StatisticsDashboardControl()
        {
            InitializeComponent();
            Loaded += StatisticsDashboardControl_Loaded;
        }

        private void StatisticsDashboardControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            using (var db = QuizDbContext.getContext())
            {
                var sessions = db.QuizSessions.Include(s => s.Category).ToList();
                
                if (!sessions.Any())
                {
                    // Handle case with no data
                    return;
                }

                DisplayGeneralStats(sessions);
                DrawWinRateChart(sessions);
                DrawCategoryPerformanceChart(sessions);
                DrawProgressChart(sessions);
            }
        }

        private void DisplayGeneralStats(List<QuizSession> sessions)
        {
            GamesPlayedText.Text = sessions.Count.ToString();
            
            double avgScore = sessions.Average(s => (double)s.Score / s.TotalQuestions * 100);
            AvgScoreText.Text = $"{avgScore:F1}%";

            double avgTimeInSeconds = sessions.Average(s => s.CompletionTime.TotalSeconds);
            AvgTimeText.Text = $"{avgTimeInSeconds:F1}s";
        }

        private void DrawWinRateChart(List<QuizSession> sessions)
        {
            WinRateCanvas.Children.Clear();
            if (!sessions.Any()) return;

            int winCount = sessions.Count(s => (double)s.Score / s.TotalQuestions > 0.5);
            double winRate = (double)winCount / sessions.Count;

            double canvasSize = WinRateCanvas.Width;
            Point center = new Point(canvasSize / 2, canvasSize / 2);
            double radius = canvasSize / 2;

            // Background for the 'loss' part
            var backgroundPath = new Path
            {
                Fill = new SolidColorBrush(Color.FromRgb(60, 60, 70)),
                Data = new EllipseGeometry(center, radius, radius)
            };
            WinRateCanvas.Children.Add(backgroundPath);

            // Foreground for the 'win' part
            if (winRate > 0)
            {
                double angle = winRate * 360;
                var winPath = new Path
                {
                    Fill = new SolidColorBrush(Color.FromRgb(75, 90, 255)),
                    Data = CreatePieSlice(center, radius, 0, angle)
                };
                WinRateCanvas.Children.Add(winPath);
            }

            // Central text
            var textBlock = new TextBlock
            {
                Text = $"{winRate:P0}",
                Foreground = Brushes.White,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            var textSize = new Size(textBlock.ActualWidth, textBlock.ActualHeight);
            Canvas.SetLeft(textBlock, center.X - textSize.Width / 2 - 15);
            Canvas.SetTop(textBlock, center.Y - textSize.Height / 2 - 15);
            WinRateCanvas.Children.Add(textBlock);
        }
        
        private PathGeometry CreatePieSlice(Point center, double radius, double startAngle, double endAngle)
        {
            var path = new PathGeometry();
            var figure = new PathFigure { StartPoint = center, IsClosed = true };
            
            var startPoint = new Point(center.X + radius * Math.Cos(startAngle * Math.PI / 180), center.Y + radius * Math.Sin(startAngle * Math.PI / 180));
            var endPoint = new Point(center.X + radius * Math.Cos(endAngle * Math.PI / 180), center.Y + radius * Math.Sin(endAngle * Math.PI / 180));

            figure.Segments.Add(new LineSegment(startPoint, true));
            figure.Segments.Add(new ArcSegment(endPoint, new Size(radius, radius), 0, (endAngle - startAngle) > 180, SweepDirection.Clockwise, true));

            path.Figures.Add(figure);
            return path;
        }

        private void DrawCategoryPerformanceChart(List<QuizSession> sessions)
        {
            CategoryPerformanceCanvas.Children.Clear();
            if (!sessions.Any()) return;

            var categoryPerformance = sessions
                .Where(s => s.Category != null)
                .GroupBy(s => s.Category!.Name)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    AverageScore = g.Average(s => (double)s.Score / s.TotalQuestions)
                })
                .OrderByDescending(x => x.AverageScore)
                .Take(5)
                .ToList();

            double canvasHeight = CategoryPerformanceCanvas.ActualHeight > 0 ? CategoryPerformanceCanvas.ActualHeight : 150;
            double canvasWidth = CategoryPerformanceCanvas.ActualWidth > 0 ? CategoryPerformanceCanvas.ActualWidth : 350;
            double barWidth = canvasWidth / (categoryPerformance.Count * 2);
            double spacing = barWidth;

            for (int i = 0; i < categoryPerformance.Count; i++)
            {
                var item = categoryPerformance[i];
                double barHeight = item.AverageScore * (canvasHeight - 20); // Leave space for labels

                var bar = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = new SolidColorBrush(Color.FromRgb(75, 90, 255)),
                    ToolTip = $"{item.CategoryName}: {item.AverageScore:P0}"
                };

                Canvas.SetLeft(bar, i * (barWidth + spacing) + spacing / 2);
                Canvas.SetTop(bar, canvasHeight - barHeight - 20);
                CategoryPerformanceCanvas.Children.Add(bar);

                var label = new TextBlock
                {
                    Text = item.CategoryName,
                    Foreground = Brushes.White,
                    FontSize = 10,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    Width = barWidth + spacing,
                    TextAlignment = TextAlignment.Center
                };

                Canvas.SetLeft(label, i * (barWidth + spacing));
                Canvas.SetTop(label, canvasHeight - 18);
                CategoryPerformanceCanvas.Children.Add(label);
            }
        }

        private void DrawProgressChart(List<QuizSession> sessions)
        {
            ProgressCanvas.Children.Clear();
            if (sessions.Count < 2) return;

            var orderedSessions = sessions.OrderBy(s => s.Date).ToList();
            double canvasHeight = ProgressCanvas.ActualHeight > 0 ? ProgressCanvas.ActualHeight : 200;
            double canvasWidth = ProgressCanvas.ActualWidth > 0 ? ProgressCanvas.ActualWidth : 750;

            double xStep = canvasWidth / (orderedSessions.Count - 1);
            
            var points = new PointCollection();
            for (int i = 0; i < orderedSessions.Count; i++)
            {
                double scorePercentage = (double)orderedSessions[i].Score / orderedSessions[i].TotalQuestions;
                points.Add(new Point(i * xStep, canvasHeight - (scorePercentage * canvasHeight)));
            }

            var polyline = new Polyline
            {
                Stroke = new SolidColorBrush(Color.FromRgb(75, 90, 255)),
                StrokeThickness = 2,
                Points = points
            };

            ProgressCanvas.Children.Add(polyline);
            
            // Add points to the line
            for (int i = 0; i < points.Count; i++)
            {
                var ellipse = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = new SolidColorBrush(Color.FromRgb(75, 90, 255)),
                    Stroke = Brushes.White,
                    StrokeThickness = 2,
                    ToolTip = $"Score: {orderedSessions[i].Score}/{orderedSessions[i].TotalQuestions} on {orderedSessions[i].Date:d}"
                };

                Canvas.SetLeft(ellipse, points[i].X - 4);
                Canvas.SetTop(ellipse, points[i].Y - 4);
                ProgressCanvas.Children.Add(ellipse);
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