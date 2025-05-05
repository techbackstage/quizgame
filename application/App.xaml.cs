using System.Windows;

namespace application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var main = new MainWindow();
            main.Show();
        }
    }
}