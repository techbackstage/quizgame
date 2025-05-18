using System.Collections.Generic;

namespace QuizGame.API
{
    public static class ParserStorage
    {
        private static List<Question> _questions = new List<Question>();
        
        public static void StoreQuestions(List<Question> questions)
        {
            _questions = questions;
        }
        
        public static List<Question> GetQuestions()
        {
            return _questions;
        }
        
        public static void Clear()
        {
            _questions.Clear();
        }
    }
} 