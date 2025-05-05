using System.Collections.Generic;

namespace QuizGame.API
{
    public class Question
    {
        public string Text { get; set; }
        public List<Answer> Answers { get; set; }
    }
}