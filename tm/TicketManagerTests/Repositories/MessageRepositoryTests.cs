using Microsoft.EntityFrameworkCore;
using TicketManager.Data;
using TicketManager.Data.Repositories;
using TicketManager.Models;

namespace TicketManagerTests.Repositories;

public class MessageRepositoryTests
{
    private readonly AppDbContext _dbContext;
    private readonly MessageRepository _messageRepository;

    public MessageRepositoryTests()
    {
        // Dependencies
        _dbContext = GetDbContext();
        // SUT
        _messageRepository = new MessageRepository(_dbContext);
    }

    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;
        var dbContext = new AppDbContext(options);
        dbContext.Database.EnsureCreated();
        if (dbContext.Messages.Count() < 0)
        {
            for (int i = 0; i < 10; i++)
            {
                dbContext.Messages.Add(new Message()
                {
                    Id = i + 1,
                    SenderName = "test name",
                    Body = "test message",
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
        var message = new Message()
        {
            Id = 1,
            SenderName = "test name",
            Body = "test message",
            Date = DateTime.MaxValue
        };
        // Act
        var result = _messageRepository.Add(message);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Delete_ReturnsTrue()
    {
        // Arrange
        var message = new Message()
        {
            Id = 1,
            SenderName = "test name",
            Body = "test message",
            Date = DateTime.MaxValue
        };
        _messageRepository.Add(message);
        // Act
        var result = _messageRepository.Delete(message);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Update_ReturnsTrue()
    {
        // Arrange
        var message = new Message()
        {
            Id = 1,
            SenderName = "test name",
            Body = "test message",
            Date = DateTime.MaxValue
        };
        _messageRepository.Add(message);
        // Act
        var result = _messageRepository.Update(message);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Save_ReturnsBool()
    {
        // Arrange (empty)
        // Act
        var result = _messageRepository.Save();
        // Assert
        Assert.IsType<bool>(result);
    }

    [Fact]
    public async void GetByIdAsync_ReturnsMessageTask()
    {
        // Arrange
        var id = 1;
        // Act
        var result = _messageRepository.GetByIdAsync(id);
        // Assert
        await Assert.IsType<Task<Message>>(result);
    }

    [Fact]
    public async void GetAllAsync_ReturnsIEnumerableMessageTask()
    {
        // Arrange (empty)
        // Act
        var result = _messageRepository.GetAllAsync();
        // Assert
        await Assert.IsType<Task<IEnumerable<Message>>>(result);
    }
}
