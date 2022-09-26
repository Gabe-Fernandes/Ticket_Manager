using Microsoft.EntityFrameworkCore;
using TicketManager.Data;
using TicketManager.Data.Repositories;
using TicketManager.Models;

namespace TicketManagerTests.Repositories;

public class TicketRepositoryTests
{
    private readonly AppDbContext _dbContext;
    private readonly TicketRepository _ticketRepository;

    public TicketRepositoryTests()
    {
        // Dependencies
        _dbContext = GetDbContext();
        // SUT
        _ticketRepository = new TicketRepository(_dbContext);
    }

    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;
        var dbContext = new AppDbContext(options);
        dbContext.Database.EnsureCreated();
        if (dbContext.Tickets.Count() < 0)
        {
            for (int i = 0; i < 10; i++)
            {
                dbContext.Tickets.Add(new Ticket()
                {
                    Id = i + 1,
                    Title = "test name",
                    Description = "test description",
                    PriorityLevel = "Medium",
                    Status = "Open",
                    StartDate = DateTime.Now.ToString(),
                    EndDate = DateTime.Now.ToString(),
                    TempDate = DateTime.Now,
                    RecipientName = "test name",
                    SenderName = "test name",
                    RecipientId = "k1",
                    SenderId = "j1",
                    ProjectId = 5
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
        var ticket = new Ticket()
        {
            Id = 1,
            Title = "test name",
            Description = "test description",
            PriorityLevel = "Medium",
            Status = "Open",
            StartDate = DateTime.Now.ToString(),
            EndDate = DateTime.Now.ToString(),
            TempDate = DateTime.Now,
            RecipientName = "test name",
            SenderName = "test name",
            RecipientId = "k1",
            SenderId = "j1",
            ProjectId = 5
        };
        // Act
        var result = _ticketRepository.Add(ticket);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Delete_ReturnsTrue()
    {
        // Arrange
        var ticket = new Ticket()
        {
            Id = 1,
            Title = "test name",
            Description = "test description",
            PriorityLevel = "Medium",
            Status = "Open",
            StartDate = DateTime.Now.ToString(),
            EndDate = DateTime.Now.ToString(),
            TempDate = DateTime.Now,
            RecipientName = "test name",
            SenderName = "test name",
            RecipientId = "k1",
            SenderId = "j1",
            ProjectId = 5
        };
        _ticketRepository.Add(ticket);
        // Act
        var result = _ticketRepository.Delete(ticket);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Update_ReturnsTrue()
    {
        // Arrange
        var ticket = new Ticket()
        {
            Id = 1,
            Title = "test name",
            Description = "test description",
            PriorityLevel = "Medium",
            Status = "Open",
            StartDate = DateTime.Now.ToString(),
            EndDate = DateTime.Now.ToString(),
            TempDate = DateTime.Now,
            RecipientName = "test name",
            SenderName = "test name",
            RecipientId = "k1",
            SenderId = "j1",
            ProjectId = 5
        };
        _ticketRepository.Add(ticket);
        // Act
        var result = _ticketRepository.Update(ticket);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Save_ReturnsBool()
    {
        // Arrange (empty)
        // Act
        var result = _ticketRepository.Save();
        // Assert
        Assert.IsType<bool>(result);
    }

    [Fact]
    public async void GetByIdAsync_ReturnsTicketTask()
    {
        // Arrange
        var id = 1;
        // Act
        var result = _ticketRepository.GetByIdAsync(id);
        // Assert
        await Assert.IsType<Task<Ticket>>(result);
    }

    [Fact]
    public async void GetAllAsync_ReturnsIEnumerableTicketTask()
    {
        // Arrange (empty)
        // Act
        var result = _ticketRepository.GetAllAsync();
        // Assert
        await Assert.IsType<Task<IEnumerable<Ticket>>>(result);
    }
}
