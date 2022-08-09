using Microsoft.EntityFrameworkCore;
using TicketManager.Interfaces;
using TicketManager.Models;

namespace TicketManager.Data.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _db;

    public TicketRepository(AppDbContext db)
    {
        _db = db;
    }

    public bool Add(Ticket ticket)
    {
        _db.Add(ticket);
        return Save();
    }

    public bool Delete(Ticket ticket)
    {
        _db.Remove(ticket);
        return Save();
    }

    public async Task<IEnumerable<Ticket>> GetAllAsync()
    {
        return await _db.Tickets.ToListAsync();
    }

    public async Task<Ticket> GetByIdAsync(int? id)
    {
        return await _db.Tickets.FindAsync(id);
    }

    public bool Save()
    {
        int numSaved = _db.SaveChanges(); // returns the number of entries written to the database
        return numSaved > 0;
    }

    public bool Update(Ticket ticket)
    {
        _db.Update(ticket);
        return Save();
    }
}
