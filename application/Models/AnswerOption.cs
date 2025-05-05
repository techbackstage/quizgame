namespace application.Models;

/// <summary>
/// Represents an answer option for a quiz question.
/// </summary>
public class AnswerOption
{
    public int AnswerOptionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int QuestionId { get; set; }
    public virtual Question Question { get; set; } = null!;
}
