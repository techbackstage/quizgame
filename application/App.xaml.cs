using System;
using System.Windows;
using System.IO;

namespace application
{
    /// <summary>
    /// The main application class. Handles startup and global exception handling.
    /// </summary>
    public partial class App : Application
    {
        // Add unhandled exception handler
        public App()
        {
            // Log to file as the console output might not be visible
            File.AppendAllText("app_log.txt", $"{DateTime.Now}: App constructor called\n");
            
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            this.Startup += App_Startup;
            this.Exit += App_Exit;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            File.AppendAllText("app_log.txt", $"{DateTime.Now}: App started\n");
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            File.AppendAllText("app_log.txt", $"{DateTime.Now}: App exited with code: {e.ApplicationExitCode}\n");
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            File.AppendAllText("app_log.txt", $"{DateTime.Now}: UNHANDLED EXCEPTION: {e.Exception.Message}\n{e.Exception.StackTrace}\n");
            MessageBox.Show("An unhandled exception occurred: " + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}