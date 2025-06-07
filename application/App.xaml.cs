// filepath: c:\Users\Erik\RiderProjects\quizgame\application\App.xaml.cs
using System;
using System.Windows;
using System.IO;
using PdfSharp.Fonts;
using QuizGame.Application.Common;

namespace QuizGame.Application
{
    /// <summary>
    /// The main application class. Handles startup and global exception handling.
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            // Set up app-level exception handling
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Log the exception
            File.AppendAllText("error_log.txt", $"{DateTime.Now}: UNHANDLED EXCEPTION: {e.Exception.Message}\n{e.Exception.StackTrace}\n");
            
            // Show error message to user
            MessageBox.Show($"An error occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            
            // Mark as handled to prevent app crash
            e.Handled = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            GlobalFontSettings.FontResolver = new MyFontResolver();
        }
    }
}
