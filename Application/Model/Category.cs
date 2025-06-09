using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace QuizGame.Application.Model;

/// <summary>
/// Represents a quiz category.
/// </summary>
public class Category : INotifyPropertyChanged
{
    private bool _isSelectedForExport;
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<Question> Questions { get; set; } = new();
    
    // Add date tracking properties
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    // Property for PDF export selection
    [NotMapped] // This property is not stored in the database
    public bool IsSelectedForExport
    {
        get => _isSelectedForExport;
        set
        {
            if (_isSelectedForExport != value)
            {
                _isSelectedForExport = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
