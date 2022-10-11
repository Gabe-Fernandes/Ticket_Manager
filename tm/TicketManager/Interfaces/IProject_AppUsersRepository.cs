using TicketManager.Models;

namespace TicketManager.Interfaces;

public interface IProject_AppUsersRepository
{
    Task<IEnumerable<Project_AppUsers>> GetAllAsync();
    Task<List<string>> GetTeamMemberIdsAsync(int projId);
    Task<List<AppUser>> GetTeamMembersAsync(int projId);
    Task<List<string>> GetIdsOfRoleAsync(int projId, string role);
    Task<List<string>> GetManagers(int projId);
    Task<List<Ticket>> GetTicketsFromProjAsync(int projId);
    Task<List<Project>> GetMyProjectsAsync(string userId);
    Task<Project_AppUsers> GetByIdAsync(int projId, string userId);
    Task<bool> IsApproved(int projId, string userId);
    bool Add(Project_AppUsers project_AppUsers);
    bool Update(Project_AppUsers project_AppUsers);
    bool Delete(Project_AppUsers project_AppUsers);
    bool Save();
}
