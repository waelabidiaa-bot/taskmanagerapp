using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Models;

namespace TaskManager.Data.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<LearningActivity> LearningActivities => Set<LearningActivity>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name).UseCollation("NOCASE");
            entity.HasIndex(c => c.Name).IsUnique();
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(t => t.LearningActivity)
                .WithMany(l => l.Tasks)
                .HasForeignKey(t => t.LearningActivityId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<LearningActivity>()
            .Ignore(l => l.CompletedTasks);

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasOne(n => n.Task)
                .WithMany(t => t.Notes)
                .HasForeignKey(n => n.TaskId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(n => n.Category)
                .WithMany(c => c.Notes)
                .HasForeignKey(n => n.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(n => n.LearningActivity)
                .WithMany(l => l.Notes)
                .HasForeignKey(n => n.LearningActivityId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
