using System;

namespace QuizGame.Application.Model;

/// <summary>
/// Represents a quiz session for tracking user results.
/// </summary>
public class QuizSession
{
    public int QuizSessionId { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public TimeSpan CompletionTime { get; set; }
    public DateTime Date { get; set; }
    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
}
