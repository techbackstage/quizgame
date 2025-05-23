using System.Collections.Generic;
using System.Text.RegularExpressions;
using QuizGame.Application.Model;
using System;

namespace QuizGame.API
{
    public class Parser
    {
        protected string rawContent = "";

        public Parser(string content)
        {
            this.rawContent = content;
        }

        public List<Question> parse()
        {
           return ParseQuestionsAndAnswers(this.rawContent);
        }

        // Methode zum Parsen der Fragen und Antworten
        protected List<Question> ParseQuestionsAndAnswers(string input)
        {
            var questions = new List<Question>();

        // Regex zum Extrahieren der Fragen (zwischen #?# und #*#) und Antworten (zwischen #-# und +# für die richtige Antwort)
        var questionRegex = new Regex(@"#\?#(.*?)#\*#", RegexOptions.Singleline);
        var answerRegex = new Regex(@"#-#(.*?)#-#", RegexOptions.Singleline);
        var correctAnswerRegex = new Regex(@"\+#(.*?)#-#", RegexOptions.Singleline);

        // Frage-Extraktion
        var questionMatches = questionRegex.Matches(input);
        
        foreach (Match questionMatch in questionMatches)
        {
            var questionText = questionMatch.Groups[1].Value.Trim();

            // Antwort-Extraktion
            var answerMatches = answerRegex.Matches(input);

            var answers = new List<AnswerOption>();
            bool correctAnswerFound = false;

            foreach (Match answerMatch in answerMatches)
            {
                var answerText = answerMatch.Groups[1].Value.Trim();
                bool isCorrect = false;

                // Prüfen, ob die Antwort korrekt ist
                if (!correctAnswerFound && correctAnswerRegex.IsMatch(answerMatch.Value))
                {
                    isCorrect = true;
                    correctAnswerFound = true;
                }

                answers.Add(new AnswerOption
                {
                    Text = answerText.Replace("#+#", ""),
                    IsCorrect = isCorrect
                });
            }

            // Eine neue Frage mit den Antworten erstellen
            questions.Add(new Question
            {
                Text = questionText,
                AnswerOptions = answers
            });
        }

        return questions;
        }
    }
}