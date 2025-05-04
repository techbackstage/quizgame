using System.Windows;

namespace ConsoleApp1
{
    public partial class Window4 : Window
    {
        public Window4()
        {
            InitializeComponent();
            this.Loaded += Window4_Loaded;
        }

        private void Window4_Loaded(object sender, RoutedEventArgs e)
        {
            AttachButtonHandler("Quiz beginnen", QuizBeginnen_Click);
        }

        private void AttachButtonHandler(string buttonText, RoutedEventHandler handler)
        {
            if (this.Content is System.Windows.FrameworkElement root)
            {
                foreach (var child in FindVisualChildren<System.Windows.Controls.Button>(root))
                {
                    if (child.Content is string s && s == buttonText)
                    {
                        child.Click += handler;
                        break;
                    }
                }
            }
        }

        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(System.Windows.DependencyObject depObj) where T : System.Windows.DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    System.Windows.DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void QuizBeginnen_Click(object sender, RoutedEventArgs e)
        {
            var win5 = new Window5();
            win5.Show();
            this.Close();
        }
    }
}
