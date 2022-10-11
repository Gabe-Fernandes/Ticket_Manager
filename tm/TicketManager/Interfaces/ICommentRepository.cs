using TicketManager.Models;

namespace TicketManager.Interfaces;

public interface ICommentRepository
{
    Task<List<Comment>> GetAllAsync(int ticketId);
    Task<Comment> GetByIdAsync(int id);
    bool Add(Comment comment);
    bool Update(Comment comment);
    bool Delete(Comment comment);
    bool Save();
}
