using ApiNetCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiNetCore.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.IsComplete).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
        });
    }
}
