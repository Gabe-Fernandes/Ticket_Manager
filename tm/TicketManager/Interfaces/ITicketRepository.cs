using TicketManager.Models;

namespace TicketManager.Interfaces;

public interface ITicketRepository
{
    Task<List<Ticket>> GetAllFromProjectAsync(int projId);
    Task<Ticket> GetByIdAsync(int id);
    bool Add(Ticket ticket);
    bool Update(Ticket ticket);
    bool Delete(Ticket ticket);
    bool Save();
}
