using System.Collections.Generic;

namespace application.Models;

/// <summary>
/// Represents a quiz category.
/// </summary>
public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<Question> Questions { get; set; } = new();
}
