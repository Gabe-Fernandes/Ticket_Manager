using Microsoft.EntityFrameworkCore;
using TicketManager.Interfaces;
using TicketManager.Models;
using TicketManager.Pages.Identity;

namespace TicketManager.Data.Repositories;

public class Project_AppUsersRepository : IProject_AppUsersRepository
{
    private readonly AppDbContext _db;

    public Project_AppUsersRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<string>> GetTeamMemberIdsAsync(int projId)
    {
        var projectIdList = await _db.Project_AppUsers.Where(p => p.ProjectId == projId).ToListAsync();
        List<string> teamMemberIds = new List<string>();
        foreach (var project in projectIdList)
        {
            teamMemberIds.Add(project.AppUserId);
        }
        return teamMemberIds;
    }

    public async Task<List<AppUser>> GetTeamMembersAsync(int projId)
    {
        List<string> teamMemberIds = await GetTeamMemberIdsAsync(projId);

        List<AppUser> teamMembers = new List<AppUser>();
        foreach (var id in teamMemberIds)
        {
            AppUser appUser = await _db.Users.FindAsync(id);
            teamMembers.Add(appUser);
        }
        return teamMembers;
    }

    public async Task<List<string>> GetIdsOfRoleAsync(int projId, string role)
    {
        var teamMembers = await GetTeamMembersAsync(projId);
        var users = teamMembers.Where(t => t.AssignedRole == role).ToList();
        List<string> idList = new List<string>();
        foreach (var user in users)
        {
            idList.Add(user.Id);
        }
        return idList;
    }

    public async Task<List<Project>> GetMyProjectsAsync(string userId)
    {
        var myProjAppUserRelations =
            await _db.Project_AppUsers.Where(pa => pa.AppUserId == userId).ToListAsync();
        List<Project> myProjects = new List<Project>();
        foreach (var relation in myProjAppUserRelations)
        {
            var proj = await _db.Projects.FindAsync(relation.ProjectId);
            myProjects.Add(proj);
        }
        return myProjects;
    }

    public async Task<List<Ticket>> GetTicketsFromProjAsync(int projId)
    {
        return await _db.Tickets.Where(t => t.ProjectId == projId).ToListAsync();
    }

    public async Task<bool> IsApproved(int projId, string userId)
    {
        Project_AppUsers project_AppUsers = await GetByIdAsync(projId, userId);
        return project_AppUsers.Approved;
    }

    public bool Add(Project_AppUsers project_AppUsers)
    {
        _db.Add(project_AppUsers);
        return Save();
    }

    public bool Delete(Project_AppUsers project_AppUsers)
    {
        _db.Remove(project_AppUsers);
        return Save();
    }

    public async Task<IEnumerable<Project_AppUsers>> GetAllAsync()
    {
        return await _db.Project_AppUsers.ToListAsync();
    }

    public async Task<Project_AppUsers> GetByIdAsync(int projId, string appUserId)
    {
        return await _db.Project_AppUsers.Where(p => p.ProjectId == projId)
            .Where(a => a.AppUserId == appUserId).FirstAsync();
   }

    public bool Save()
    {
        int numSaved = _db.SaveChanges(); // returns the number of entries written to the database
        return numSaved > 0;
    }

    public bool Update(Project_AppUsers project_AppUsers)
    {
        _db.Update(project_AppUsers);
        return Save();
    }
}
