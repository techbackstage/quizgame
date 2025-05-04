using System.Windows;

namespace ConsoleApp1
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            this.Loaded += Window1_Loaded;
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            // Attach click event directly by name for better reliability
            if (FindName("KategorienButton") is System.Windows.Controls.Button btn)
            {
                btn.Click += KategorienVerwalten_Click;
            }
        }

        private void KategorienVerwalten_Click(object sender, RoutedEventArgs e)
        {
            var win2 = new Window2();
            var win3 = new Window3();
            win2.Show();
            win3.Show();
            this.Close();

        }

        // Add event handlers for the other buttons
        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);
            // "Quiz starten" button (first button)
            if (this.Content is System.Windows.FrameworkElement root)
            {
                var stack = root.FindName("KategorienButton") as System.Windows.Controls.Button;
                // Already handled above
            }
            // Attach by visual tree for other buttons
            AttachButtonHandler("Quiz starten", QuizStarten_Click);
            AttachButtonHandler("Statistik ansehen", StatistikAnsehen_Click);
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

        private void QuizStarten_Click(object sender, RoutedEventArgs e)
        {
            var win4 = new Window4();
            win4.Show();
            this.Close();
        }

        private void StatistikAnsehen_Click(object sender, RoutedEventArgs e)
        {
            var win7 = new Window7();
            win7.Show();
            this.Close();
        }
        }
    }

