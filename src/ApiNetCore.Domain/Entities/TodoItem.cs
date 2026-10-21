namespace ApiNetCore.Domain.Entities;

public sealed class TodoItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public bool IsComplete { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; } = DateTimeOffset.UtcNow;

    private TodoItem() { }

    public TodoItem(string title)
    {
        SetTitle(title);
    }

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        if (title.Length > 200) throw new ArgumentOutOfRangeException(nameof(title), "Title must be 200 characters or fewer.");
        Title = title.Trim();
    }

    public void MarkComplete() => IsComplete = true;
}
