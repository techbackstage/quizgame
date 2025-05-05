using System.Collections.Generic;

namespace application.Models;

/// <summary>
/// Represents a quiz question.
/// </summary>
public class Question
{
    public int QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int DifficultyLevel { get; set; }
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
    public virtual List<AnswerOption> AnswerOptions { get; set; } = new();
}