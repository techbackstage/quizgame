using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using QuizGame.Application.Model;

namespace QuizGame.Application.Database;

public class QuizDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<AnswerOption> AnswerOptions { get; set; }
    public DbSet<QuizSession> QuizSessions { get; set; }

    public static QuizDbContext GetContext()
    {
        var options = new DbContextOptionsBuilder<QuizDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=QuizDb;Trusted_Connection=True;")
                .Options;

        return new QuizDbContext(options);
    }

    public QuizDbContext(DbContextOptions<QuizDbContext> options)
       : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Question>()
            .HasMany(q => q.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}