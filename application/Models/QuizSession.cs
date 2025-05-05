using System;

namespace application.Models;

public class QuizSession
{
    public int QuizSessionId { get; set; }
    public int Score { get; set; }
    public TimeSpan CompletionTime { get; set; }
    public DateTime Date { get; set; }
}
