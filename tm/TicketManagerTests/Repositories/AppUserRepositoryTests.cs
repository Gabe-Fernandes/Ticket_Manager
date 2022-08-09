using Microsoft.EntityFrameworkCore;
using TicketManager.Data;
using TicketManager.Data.Repositories;
using TicketManager.Models;

namespace TicketManagerTests.Repositories;

public class AppUserRepositoryTests
{
    private readonly AppDbContext _dbContext;
    private readonly AppUserRepository _appUserRepository;

    public AppUserRepositoryTests()
    {
        // Dependencies
        _dbContext = GetDbContext();
        // SUT
        _appUserRepository = new AppUserRepository(_dbContext);
    }

    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;
        var dbContext = new AppDbContext(options);
        dbContext.Database.EnsureCreated();
        if (dbContext.Users.Count() < 0)
        {
            for (int i = 0; i < 10; i++)
            {
                dbContext.Users.Add(new AppUser()
                {
                    Id = $"{i + 1}",
                    FirstName = "test name",
                    LastName = "test name",
                    Email = "test@email.com",
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
        var appUser = new AppUser()
        {
            Id = "1",
            FirstName = "test name",
            LastName = "test name",
            Email = "test@email.com",
        };
        // Act
        var result = _appUserRepository.Add(appUser);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Delete_ReturnsTrue()
    {
        // Arrange
        var appUser = new AppUser()
        {
            Id = "1",
            FirstName = "test name",
            LastName = "test name",
            Email = "test@email.com",
        };
        _appUserRepository.Add(appUser);
        // Act
        var result = _appUserRepository.Delete(appUser);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Update_ReturnsTrue()
    {
        // Arrange
        var appUser = new AppUser()
        {
            Id = "1",
            FirstName = "test name",
            LastName = "test name",
            Email = "test@email.com",
        };
        _appUserRepository.Add(appUser);
        // Act
        var result = _appUserRepository.Update(appUser);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Save_ReturnsBool()
    {
        // Arrange (empty)
        // Act
        var result = _appUserRepository.Save();
        // Assert
        Assert.IsType<bool>(result);
    }

    [Fact]
    public async void GetByIdAsync_ReturnsAppUserTask()
    {
        // Arrange
        string id = "1";
        // Act
        var result = _appUserRepository.GetByIdAsync(id);
        // Assert
        await Assert.IsType<Task<AppUser>>(result);
    }

    [Fact]
    public async void GetAllAsync_ReturnsIEnumerableAppUserTask()
    {
        // Arrange (empty)
        // Act
        var result = _appUserRepository.GetAllAsync();
        // Assert
        await Assert.IsType<Task<IEnumerable<AppUser>>>(result);
    }
}
