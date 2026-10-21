using ApiNetCore.Application.Abstractions;
using ApiNetCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiNetCore.Infrastructure.Persistence;

public sealed class TodoRepository(AppDbContext dbContext) : ITodoRepository
{
    public async Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.TodoItems.AsNoTracking().OrderBy(x => x.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.TodoItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(TodoItem item, CancellationToken cancellationToken = default) =>
        await dbContext.TodoItems.AddAsync(item, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
