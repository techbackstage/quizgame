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
            const string answerSeparator = "~|~";
            const string correctAnswerPrefix = "[CORRECT]";

            try
            {
                var questionBlocks = input.Split(new string[] { "#?#" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var block in questionBlocks)
                {
                    if (string.IsNullOrWhiteSpace(block)) continue;

                    var parts = block.Split(new string[] { "#*#" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 2) continue;

                    var questionText = parts[0].Trim();
                    var answersText = parts[1].Trim(); // Trim the whole answers string once

                    var answers = new List<AnswerOption>();
                    var answerParts = answersText.Split(new string[] { answerSeparator }, StringSplitOptions.None); // Don't remove empty entries yet, handle after trim

                    foreach (var answerPart in answerParts)
                    {
                        var answerText = answerPart.Trim();
                        if (string.IsNullOrWhiteSpace(answerText)) continue;

                        bool isCorrect = false;
                        
                        if (answerText.StartsWith(correctAnswerPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            isCorrect = true;
                            // Remove the prefix to get the actual answer text
                            answerText = answerText.Substring(correctAnswerPrefix.Length).Trim();
                        }

                        // Ensure answerText is not empty after potentially removing prefix
                        if (string.IsNullOrWhiteSpace(answerText)) continue;

                        answers.Add(new AnswerOption
                        {
                            Text = answerText,
                            IsCorrect = isCorrect
                        });
                    }

                    if (answers.Count > 0) // Ensure there are answers, ideally 4, but at least 1
                    {
                        // Optional: Add validation here to ensure exactly one answer is marked correct if required by your logic
                        /*
                        if (answers.Count(a => a.IsCorrect) != 1 && answers.Any()) 
                        {
                            Console.WriteLine($"Warning: Question '{questionText}' does not have exactly one correct answer. Proceeding, but this might be an issue.");
                            // Depending on requirements, you might skip this question or try to fix it
                        }
                        */
                        questions.Add(new Question
                        {
                            Text = questionText,
                            AnswerOptions = answers
                            // DifficultyLevel and CategoryId would need to be parsed if present in 'block'
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing questions: {ex.Message}");
                // Potentially log to a file or re-throw if critical
            }

            return questions;
        }
    }
}