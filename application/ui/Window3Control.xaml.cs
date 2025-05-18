
using System.Windows;
using System.Windows.Controls;
using application.Data;
using application.Models;

namespace application
{
    public partial class Window3Control : UserControl
    {
        public Window3Control()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var name = CategoryNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Bitte einen Kategorienamen eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            using (var db = new QuizDbContext())
            {
                if (db.Categories.Any(c => c.Name == name))
                {
                    MessageBox.Show("Kategorie existiert bereits.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                db.Categories.Add(new Category { Name = name });
                db.SaveChanges();
            }
            // Close parent window if hosted in a Window
            Window.GetWindow(this)?.Close();
        }
    }
}
