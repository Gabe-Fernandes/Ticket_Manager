using Microsoft.EntityFrameworkCore;
using TicketManager.Interfaces;
using TicketManager.Models;

namespace TicketManager.Data.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _db;

    public CommentRepository(AppDbContext db)
    {
        _db = db;
    }

    public bool Add(Comment comment)
    {
        _db.Add(comment);
        return Save();
    }

    public bool Delete(Comment comment)
    {
        _db.Remove(comment);
        return Save();
    }

    public async Task<List<Comment>> GetAllAsync(int ticketId)
    {
        return await _db.Comments.Where(c => c.TicketId == ticketId).ToListAsync();
    }

    public async Task<Comment> GetByIdAsync(int id)
    {
        return await _db.Comments.FindAsync(id);
    }

    public bool Save()
    {
        int numSaved = _db.SaveChanges(); // returns the number of entries written to the database
        return numSaved > 0;
    }

    public bool Update(Comment comment)
    {
        _db.Update(comment);
        return Save();
    }
}
