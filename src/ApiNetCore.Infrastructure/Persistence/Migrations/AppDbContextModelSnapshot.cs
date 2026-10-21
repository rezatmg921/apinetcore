using ApiNetCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace ApiNetCore.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
public sealed class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        modelBuilder.Entity("ApiNetCore.Domain.Entities.TodoItem", entity =>
        {
            entity.Property<Guid>("Id").HasColumnType("uniqueidentifier");
            entity.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("datetimeoffset");
            entity.Property<bool>("IsComplete").HasColumnType("bit");
            entity.Property<string>("Title").IsRequired().HasMaxLength(200).HasColumnType("nvarchar(200)");
            entity.HasKey("Id");
            entity.ToTable("TodoItems");
        });
    }
}
