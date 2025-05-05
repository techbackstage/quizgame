using System.Collections.Generic;

namespace application.Models;

public class Question
{
    public int QuestionId { get; set; }
    public string Text { get; set; }
    public int DifficultyLevel { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<AnswerOption> AnswerOptions { get; set; } = new();
}