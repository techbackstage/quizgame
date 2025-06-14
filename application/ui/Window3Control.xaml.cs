using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuizGame.Application.Database;
using QuizGame.Application.Model;

namespace QuizGame.Application.UI
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
            using (var db = QuizDbContext.getContext())
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
