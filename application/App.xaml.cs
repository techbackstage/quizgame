using System.Windows;
using ConsoleApp1;

namespace application;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var w1 = new Window1();
        w1.Show();
    }
}