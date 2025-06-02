using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizGame.Application.Model;

/// <summary>
/// Represents a quiz category.
/// </summary>
public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<Question> Questions { get; set; } = new();
    
    // Add date tracking properties
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    // Property for PDF export selection
    [NotMapped] // This property is not stored in the database
    public bool IsSelectedForExport { get; set; }
}
