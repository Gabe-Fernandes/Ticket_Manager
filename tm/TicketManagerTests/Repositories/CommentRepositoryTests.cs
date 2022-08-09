using Microsoft.EntityFrameworkCore;
using TicketManager.Data;
using TicketManager.Data.Repositories;
using TicketManager.Models;

namespace TicketManagerTests.Repositories;

public class CommentRepositoryTests
{
    private readonly AppDbContext _dbContext;
    private readonly CommentRepository _commentRepository;

    public CommentRepositoryTests()
    {
        // Dependencies
        _dbContext = GetDbContext();
        // SUT
        _commentRepository = new CommentRepository(_dbContext);
    }

    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;
        var dbContext = new AppDbContext(options);
        dbContext.Database.EnsureCreated();
        if (dbContext.Comments.Count() < 0)
        {
            for (int i = 0; i < 10; i++)
            {
                dbContext.Comments.Add(new Comment()
                {
                    Id = i + 1,
                    Name = "test name",
                    Body = "test comment",
                    Date = DateTime.MaxValue
                });
                dbContext.SaveChangesAsync();
            }
        }
        return dbContext;
    }

    [Fact]
    public void Add_ReturnsTrue()
    {
        // Arrange
        var comment = new Comment()
        {
            Id = 1,
            Name = "test name",
            Body = "test comment",
            Date = DateTime.MaxValue
        };
        // Act
        var result = _commentRepository.Add(comment);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Delete_ReturnsTrue()
    {
        // Arrange
        var comment = new Comment()
        {
            Id = 1,
            Name = "test name",
            Body = "test comment",
            Date = DateTime.MaxValue
        };
        _commentRepository.Add(comment);
        // Act
        var result = _commentRepository.Delete(comment);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Update_ReturnsTrue()
    {
        // Arrange
        var comment = new Comment()
        {
            Id = 1,
            Name = "test name",
            Body = "test comment",
            Date = DateTime.MaxValue
        };
        _commentRepository.Add(comment);
        // Act
        var result = _commentRepository.Update(comment);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Save_ReturnsBool()
    {
        // Arrange (empty)
        // Act
        var result = _commentRepository.Save();
        // Assert
        Assert.IsType<bool>(result);
    }

    [Fact]
    public async void GetByIdAsync_ReturnsCommentTask()
    {
        // Arrange
        var id = 1;
        // Act
        var result = _commentRepository.GetByIdAsync(id);
        // Assert
        await Assert.IsType<Task<Comment>>(result);
    }

    [Fact]
    public async void GetAllAsync_ReturnsIEnumerableCommentTask()
    {
        // Arrange (empty)
        // Act
        var result = _commentRepository.GetAllAsync();
        // Assert
        await Assert.IsType<Task<IEnumerable<Comment>>>(result);
    }
}
