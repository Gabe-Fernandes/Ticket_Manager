using TicketManager.Models;

namespace TicketManager.Interfaces;

public interface IMessageRepository
{
    Task<List<Message>> GetAllFromProjAsync(int projId);
    Task<Message> GetByIdAsync(int id);
    bool Add(Message message);
    bool Update(Message message);
    bool Delete(Message message);
    bool Save();
}
