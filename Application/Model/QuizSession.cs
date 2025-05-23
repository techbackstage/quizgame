using System;

namespace QuizGame.Application.Model;

/// <summary>
/// Represents a quiz session for tracking user results.
/// </summary>
public class QuizSession
{
    public int QuizSessionId { get; set; }
    public int Score { get; set; }
    public TimeSpan CompletionTime { get; set; }
    public DateTime Date { get; set; }
}
