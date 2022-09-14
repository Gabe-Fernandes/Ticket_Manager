using Microsoft.EntityFrameworkCore;
using TicketManager.Data;
using TicketManager.Data.Repositories;
using TicketManager.Models;

namespace TicketManagerTests.Repositories;

public class ProjectRepositoryTests
{
    private readonly AppDbContext _dbContext;
    private readonly ProjectRepository _projectRepository;

    public ProjectRepositoryTests()
    {
        // Dependencies
        _dbContext = GetDbContext();
        // SUT
        _projectRepository = new ProjectRepository(_dbContext);
    }

    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;
        var dbContext = new AppDbContext(options);
        dbContext.Database.EnsureCreated();
        if (dbContext.Projects.Count() < 0)
        {
            for (int i = 0; i < 10; i++)
            {
                dbContext.Projects.Add(new Project()
                {
                    Id = i + 1,
                    Name = "test name",
                    GitHubLink = "testlink",
                    StartDate = "testDate",
                    EndDate = "testDate"
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
        var project = new Project()
        {
            Id = 1,
            Name = "test name",
            GitHubLink = "testlink",
            StartDate = "testDate",
            EndDate = "testDate"
        };
        // Act
        var result = _projectRepository.Add(project);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Delete_ReturnsTrue()
    {
        // Arrange
        var project = new Project()
        {
            Id = 1,
            Name = "test name",
            GitHubLink = "testlink",
            StartDate = "testDate",
            EndDate = "testDate"
        };
        _projectRepository.Add(project);
        // Act
        var result = _projectRepository.Delete(project);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Update_ReturnsTrue()
    {
        // Arrange
        var project = new Project()
        {
            Id = 1,
            Name = "test name",
            GitHubLink = "testlink",
            StartDate = "testDate",
            EndDate = "testDate"
        };
        _projectRepository.Add(project);
        // Act
        var result = _projectRepository.Update(project);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Save_ReturnsBool()
    {
        // Arrange (empty)
        // Act
        var result = _projectRepository.Save();
        // Assert
        Assert.IsType<bool>(result);
    }

    [Fact]
    public async void GetByIdAsync_ReturnsProjectTask()
    {
        // Arrange
        var id = 1;
        // Act
        var result = _projectRepository.GetByIdAsync(id);
        // Assert
        await Assert.IsType<Task<Project>>(result);
    }

    [Fact]
    public async void GetAllAsync_ReturnsIEnumerableProjectTask()
    {
        // Arrange (empty)
        // Act
        var result = _projectRepository.GetAllAsync();
        // Assert
        await Assert.IsType<Task<IEnumerable<Project>>>(result);
    }
}
