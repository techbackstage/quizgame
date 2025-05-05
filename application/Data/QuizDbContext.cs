using Microsoft.EntityFrameworkCore;
using application.Models;

namespace application.Data;

public class QuizDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<AnswerOption> AnswerOptions { get; set; }
    public DbSet<QuizSession> QuizSessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Use absolute path to ensure correct database file is used
        var dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "quiz.db");
        var fullPath = System.IO.Path.GetFullPath(dbPath);
        options.UseSqlite($"Data Source={fullPath}");
    }
}