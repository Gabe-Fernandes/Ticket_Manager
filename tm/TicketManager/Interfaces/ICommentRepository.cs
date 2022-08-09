using TicketManager.Models;

namespace TicketManager.Interfaces;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetAllAsync();
    Task<Comment> GetByIdAsync(int? id);
    bool Add(Comment comment);
    bool Update(Comment comment);
    bool Delete(Comment comment);
    bool Save();
}
