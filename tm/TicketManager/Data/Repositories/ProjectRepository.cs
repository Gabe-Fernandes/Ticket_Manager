using Microsoft.EntityFrameworkCore;
using TicketManager.Interfaces;
using TicketManager.Models;

namespace TicketManager.Data.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _db;

    public ProjectRepository(AppDbContext db)
    {
        _db = db;
    }

    public bool Add(Project project)
    {
        _db.Add(project);
        return Save();
    }

    public bool Delete(Project project)
    {
        _db.Remove(project);
        return Save();
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _db.Projects.ToListAsync();
    }

    public async Task<Project> GetByIdAsync(int? id)
    {
        return await _db.Projects.FindAsync(id);
    }

    public bool Save()
    {
        int numSaved = _db.SaveChanges(); // returns the number of entries written to the database
        return numSaved > 0;
    }

    public bool Update(Project project)
    {
        _db.Update(project);
        return Save();
    }
}
