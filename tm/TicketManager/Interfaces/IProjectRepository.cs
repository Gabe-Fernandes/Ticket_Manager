using TicketManager.Models;

namespace TicketManager.Interfaces;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project> GetByIdAsync(int? id);
    bool Add(Project project);
    bool Update(Project project);
    bool Delete(Project project);
    bool Save();
}
