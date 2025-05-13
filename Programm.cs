// filepath: c:\Users\Erik\RiderProjects\quizgame\Programm.cs
using System;
using System.Windows;
using System.IO;

namespace QuizGame
{
    public static class Programm
    {
        [STAThread]
        public static void Main()
        {
            try
            {
                // Create a log file for debugging
                File.WriteAllText("debug_log.txt", $"{DateTime.Now}: Starting Quiz App...\n");
                
                // Create the application instance
                var app = new application.App();
                
                // Create the main window explicitly - this is key for visibility
                var mainWindow = new application.MainWindow();
                
                // Add window loaded event to log when the window actually appears
                mainWindow.Loaded += (s, e) =>
                {
                    File.AppendAllText("debug_log.txt", $"{DateTime.Now}: MainWindow loaded successfully.\n");
                    
                    // Force window to be visible by briefly making it topmost
                    mainWindow.Topmost = true;
                    
                    // Reset topmost after a short delay
                    var timer = new System.Windows.Threading.DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += (sender, args) =>
                    {
                        mainWindow.Topmost = false;
                        timer.Stop();
                    };
                    timer.Start();
                };
                
                // Make sure window is properly configured for visibility
                mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mainWindow.ShowInTaskbar = true;
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.Visibility = Visibility.Visible;
                
                // Show the window and run the application
                mainWindow.Show();
                File.AppendAllText("debug_log.txt", $"{DateTime.Now}: MainWindow.Show() called\n");
                
                // Run the application with the main window
                app.Run(mainWindow);
                File.AppendAllText("debug_log.txt", $"{DateTime.Now}: Application exited normally\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug_log.txt", $"{DateTime.Now}: ERROR: {ex.Message}\n{ex.StackTrace}\n");
                MessageBox.Show($"Error starting application: {ex.Message}\n\nSee debug_log.txt for details.", 
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
