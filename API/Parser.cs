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

            try
            {
                // Split the input into question blocks using the separator #?#
                var questionBlocks = input.Split(new string[] { "#?#" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var block in questionBlocks)
                {
                    if (string.IsNullOrWhiteSpace(block)) continue;

                    // Split each block into question part and answers part using #*#
                    var parts = block.Split(new string[] { "#*#" }, StringSplitOptions.RemoveEmptyEntries);
                    
                    if (parts.Length < 2) continue;

                    var questionText = parts[0].Trim();
                    var answersText = parts[1];

                    var answers = new List<AnswerOption>();

                    // Parse answers - split by #-# for wrong answers and #+# for correct answers
                    var answerParts = answersText.Split(new string[] { "#-#", "#+#" }, StringSplitOptions.RemoveEmptyEntries);
                    
                    // Find correct answer markers
                    var correctAnswerMatches = Regex.Matches(answersText, @"#+#(.*?)#+#");
                    
                    foreach (var answerPart in answerParts)
                    {
                        var answerText = answerPart.Trim();
                        if (string.IsNullOrWhiteSpace(answerText)) continue;

                        bool isCorrect = false;
                        
                        // Check if this answer is marked as correct
                        foreach (Match match in correctAnswerMatches)
                        {
                            if (match.Groups[1].Value.Trim().Equals(answerText, StringComparison.OrdinalIgnoreCase))
                            {
                                isCorrect = true;
                                break;
                            }
                        }

                        answers.Add(new AnswerOption
                        {
                            Text = answerText,
                            IsCorrect = isCorrect
                        });
                    }

                    // Only add questions that have at least one answer
                    if (answers.Count > 0)
                    {
                        questions.Add(new Question
                        {
                            Text = questionText,
                            AnswerOptions = answers
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing questions: {ex.Message}");
            }

            return questions;
        }
    }
}