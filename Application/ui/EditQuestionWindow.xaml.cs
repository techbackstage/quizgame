using System.Windows;
using System.Collections.ObjectModel;
using QuizGame.Application.Model;

namespace QuizGame.Application.UI
{
    public partial class EditQuestionWindow : Window
    {
        public string QuestionText { get; private set; } = string.Empty;
        public string Explanation { get; private set; } = string.Empty;
        public ObservableCollection<AnswerOption> Answers { get; } = new ObservableCollection<AnswerOption>();
        
        public EditQuestionWindow(Question question)
        {
            InitializeComponent();
            QuestionTextBox.Text = question.Text;
            ExplanationTextBox.Text = question.Explanation;
            
            foreach (var answer in question.Answers)
            {
                Answers.Add(new AnswerOption 
                { 
                    Text = answer.Text, 
                    IsCorrect = answer.IsCorrect 
                });
            }
            
            AnswersList.ItemsSource = Answers;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            QuestionText = QuestionTextBox.Text;
            Explanation = ExplanationTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 