using TicketManager.Models;

namespace TicketManager.Interfaces;

public interface ITicketRepository
{
    Task<IEnumerable<Ticket>> GetAllAsync();
    Task<Ticket> GetByIdAsync(int? id);
    bool Add(Ticket ticket);
    bool Update(Ticket ticket);
    bool Delete(Ticket ticket);
    bool Save();
}
