using TicketManager.Models;

namespace TicketManager.Interfaces;

public interface IProject_AppUsersRepository
{
    Task<IEnumerable<Project_AppUsers>> GetAllAsync();
    Task<List<string>> GetTeamMembersAsync(int projId);
    Task<List<Project>> GetMyProjectsAsync(string userId);
    Task<Project_AppUsers> GetByIdAsync(int projId, string userId);
    Task<bool> IsApproved(int projId, string userId);
    bool Add(Project_AppUsers project_AppUsers);
    bool Update(Project_AppUsers project_AppUsers);
    bool Delete(Project_AppUsers project_AppUsers);
    bool Save();
}
