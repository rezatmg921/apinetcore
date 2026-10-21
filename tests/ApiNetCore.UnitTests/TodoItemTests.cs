using ApiNetCore.Domain.Entities;

namespace ApiNetCore.UnitTests;

public sealed class TodoItemTests
{
    [Fact]
    public void Constructor_TrimsTitle_AndCreatesId()
    {
        var item = new TodoItem("  learn .NET  ");

        Assert.Equal("learn .NET", item.Title);
        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.False(item.IsComplete);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_RejectsBlankTitle(string title)
    {
        Assert.Throws<ArgumentException>(() => new TodoItem(title));
    }

    [Fact]
    public void Constructor_RejectsTitleLongerThan200Characters()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TodoItem(new string('x', 201)));
    }

    [Fact]
    public void MarkComplete_SetsCompletionFlag()
    {
        var item = new TodoItem("write tests");

        item.MarkComplete();

        Assert.True(item.IsComplete);
    }
}
