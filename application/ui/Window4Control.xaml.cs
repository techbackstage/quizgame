using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using QuizGame.Application.Database;
using QuizGame.Application.Model;

namespace QuizGame.Application.UI
{
    public partial class Window4Control : UserControl
    {
        public ObservableCollection<Category> Categories { get; set; } = new();
        public Category? SelectedCategory { get; set; }

        public Window4Control()
        {
            InitializeComponent();
            DataContext = this;
            LoadCategories();
            
            // Select the first category if available
            if (Categories.Count > 0)
            {
                CategoryComboBox.SelectedIndex = 0;
                SelectedCategory = Categories[0];
            }
            
            // Update selected category when selection changes
            CategoryComboBox.SelectionChanged += (s, e) => 
            {
                SelectedCategory = CategoryComboBox.SelectedItem as Category;
            };
        }
        
        private void LoadCategories()
        {
            Categories.Clear();
            using (var db = QuizDbContext.GetContext())
            {
                foreach (var cat in db.Categories.OrderBy(c => c.Name).ToList())
                {
                    Categories.Add(cat);
                }
            }
        }
    }
}
