using Microsoft.EntityFrameworkCore;
using TicketManager.Interfaces;
using TicketManager.Models;

namespace TicketManager.Data.Repositories;

public class AppUserRepository : IAppUserRepository
{
    private readonly AppDbContext _db;

    public AppUserRepository(AppDbContext db)
    {
        _db = db;
    }

    public bool Add(AppUser appUser)
    {
        _db.Add(appUser);
        return Save();
    }

    public bool Delete(AppUser appUser)
    {
        _db.Remove(appUser);
        return Save();
    }

    public async Task<IEnumerable<AppUser>> GetAllAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<AppUser> GetByIdAsync(string id)
    {
        return await _db.Users.FindAsync(id);
    }

    public bool Save()
    {
        int numSaved = _db.SaveChanges(); // returns the number of entries written to the database
        return numSaved > 0;
    }

    public bool Update(AppUser appUser)
    {
        _db.Update(appUser);
        return Save();
    }
}
