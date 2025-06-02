using System.Collections.Generic;
using System;

namespace QuizGame.Application.Model;

/// <summary>
/// Represents a quiz question.
/// </summary>
public class Question
{
    public int QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty; // Optional: Explanation for the correct answer
    public int DifficultyLevel { get; set; }
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
    public virtual List<AnswerOption> Answers { get; set; } = new(); // Renamed/Ensured this is the correct collection
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}