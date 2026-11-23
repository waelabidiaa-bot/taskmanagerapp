using Microsoft.EntityFrameworkCore;

namespace TaskManager.Data.Database;

public static class DatabaseConfiguration
{
    // Kept the same file name and relative path as the original project so existing data is used.
    public const string ConnectionString = "Data Source=tasks.db";

    public static void Configure(DbContextOptionsBuilder options)
    {
        options.UseSqlite(ConnectionString);
    }

    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();
    }
}
