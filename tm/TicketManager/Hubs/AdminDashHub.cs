using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using TicketManager.Interfaces;
using TicketManager.Models;
using TicketManager.Pages.Identity;

namespace TicketManager.Hubs;

public class AdminDashHub : Hub<IAdminDashHub>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProject_AppUsersRepository _paRepo;
    private readonly IAppUserRepository _appUserRepo;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly AppUser _user;

    public AdminDashHub(IProject_AppUsersRepository paRepo,
        IHttpContextAccessor contextAccessor,
        IAppUserRepository appUserRepo,
        IProjectRepository projectRepository)
    {
        _paRepo = paRepo;
        _contextAccessor = contextAccessor;
        _appUserRepo = appUserRepo;
        _user = GetUser();
        _projectRepository = projectRepository;
    }

    public async Task LoadTeamMembers()
    {
        List<MemberCtx> memberCtxList = new List<MemberCtx>();
        List<AppUser> teamMembers = await _paRepo.GetTeamMembersAsync(_user.CurrentProjectId);

        foreach (var teammember in teamMembers)
        {
            Project_AppUsers PA = await _paRepo.GetByIdAsync(_user.CurrentProjectId, teammember.Id);
            MemberCtx memberCtx = new MemberCtx(teammember, PA);
            memberCtxList.Add(memberCtx);
        }

        await Clients.Caller.GetTeamMembers(memberCtxList);
    }

    public async Task ApprovePendingMember(string appUserId)
    {
        AppUser pendingMember = await _appUserRepo.GetByIdAsync(appUserId);
        Project_AppUsers PA = await _paRepo.GetByIdAsync(_user.CurrentProjectId, pendingMember.Id);
        PA.Approved = true;
        _paRepo.Update(PA);

        MemberCtx memberCtx = new MemberCtx(pendingMember, PA);
        List<string> adminIdList =
            await _paRepo.GetIdsOfRoleAsync(_user.CurrentProjectId, LoginModel.Admin);
        await Clients.Users(adminIdList).AddTeamMember(memberCtx);
    }

    public async Task RemoveMember(string appUserId)
    {
        Project thisProject = await _projectRepository.GetByIdAsync(_user.CurrentProjectId);
        thisProject.TeamMemberCount--;
        _projectRepository.Update(thisProject);

        AppUser removedMember = await _appUserRepo.GetByIdAsync(appUserId);
        Project_AppUsers PA = await _paRepo.GetByIdAsync(_user.CurrentProjectId, removedMember.Id);
        _paRepo.Delete(PA);
    }

    private AppUser GetUser()
    {
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");
        return _appUserRepo.GetById(myId);
    }
}

public class MemberCtx
{
    public MemberCtx(AppUser appUser, Project_AppUsers pa)
    {
        AppUserId = appUser.Id;
        Pfp = appUser.ProfilePicture;
        FirstName = appUser.FirstName;
        LastName = appUser.LastName;
        Role = appUser.AssignedRole;
        ApproveBtnId = Guid.NewGuid().ToString();
        DenyBtnId = Guid.NewGuid().ToString();
        RemoveBtnId = Guid.NewGuid().ToString();
        IsApproved = pa.Approved;
    }

    public string AppUserId { get; }
    public string Pfp { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Role { get; }
    public string ApproveBtnId { get; }
    public string DenyBtnId { get; }
    public string RemoveBtnId { get; }
    public bool IsApproved { get; }
}
