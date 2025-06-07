using System.ComponentModel;
using QuizGame.Application.Model;

namespace QuizGame.Application.ViewModels
{
    public class AnswerViewModel : INotifyPropertyChanged
    {
        private readonly AnswerOption _answer;
        private bool _isSelected;
        private bool _isRevealed;

        public AnswerViewModel(AnswerOption answer)
        {
            _answer = answer;
        }

        public string Text => _answer.Text;
        public bool IsCorrect => _answer.IsCorrect;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public bool IsRevealed
        {
            get => _isRevealed;
            set
            {
                _isRevealed = value;
                OnPropertyChanged(nameof(IsRevealed));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 