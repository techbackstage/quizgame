using System.Collections.Generic;

namespace application.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Question> Questions { get; set; } = new();
}
