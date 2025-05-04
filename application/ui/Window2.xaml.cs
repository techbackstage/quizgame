using System.Windows;

namespace ConsoleApp1
{
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            this.Closed += Window2_Closed;
        }

        private void Window2_Closed(object sender, System.EventArgs e)
        {
            // If Window3 is open, close it as well
            foreach (Window w in System.Windows.Application.Current.Windows)
            {
                if (w is Window3)
                {
                    w.Close();
                    break;
                }
            }
        }
    }
}
