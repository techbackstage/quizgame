using System.Windows;

namespace application
{
    /// <summary>
    /// The main application class. Handles startup and global exception handling.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Handles application startup. Shows the main window and handles any startup errors.
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                var main = new MainWindow();
                main.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"An error occurred during application startup: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(-1);
            }
        }
    }
}