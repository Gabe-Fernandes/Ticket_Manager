using Microsoft.EntityFrameworkCore;
using TicketManager.Interfaces;
using TicketManager.Models;

namespace TicketManager.Data.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _db;

    public MessageRepository(AppDbContext db)
    {
        _db = db;
    }

    public bool Add(Message message)
    {
        _db.Add(message);
        return Save();
    }

    public bool Delete(Message message)
    {
        _db.Remove(message);
        return Save();
    }

    public async Task<List<Message>> GetAllFromProjAsync(int projId)
    {
        return await _db.Messages.Where(m => m.ProjectId == projId).ToListAsync();
    }

    public async Task<Message> GetByIdAsync(int id)
    {
        return await _db.Messages.FindAsync(id);
    }

    public bool Save()
    {
        int numSaved = _db.SaveChanges(); // returns the number of entries written to the database
        return numSaved > 0;
    }

    public bool Update(Message message)
    {
        _db.Update(message);
        return Save();
    }
}
