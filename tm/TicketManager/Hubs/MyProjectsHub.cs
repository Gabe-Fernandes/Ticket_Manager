using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TicketManager.Interfaces;
using TicketManager.Models;

namespace TicketManager.Hubs;

[Authorize]
public class MyProjectsHub : Hub<IMyProjectsHub>
{
    private readonly IProject_AppUsersRepository _project_AppUserRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IAppUserRepository _appUserRepo;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly AppUser _user;

    public MyProjectsHub(
        IAppUserRepository appUserRepo,
        IHttpContextAccessor contextAccessor,
        IProjectRepository projectRepository,
        IProject_AppUsersRepository projectAppUserRepository)
    {
        _appUserRepo = appUserRepo;
        _contextAccessor = contextAccessor;
        _projectRepository = projectRepository;
        _user = GetUser();
        _project_AppUserRepository = projectAppUserRepository;
    }

    public async Task CreateProject(object formData)
    {
        string formDataString = formData.ToString();
        ProjectDataFromClient projectDataFromClient =
            JsonConvert.DeserializeObject<ProjectDataFromClient>(formDataString);

        Project project = new Project
        {
            Name = projectDataFromClient.Name,
            GitHubLink = projectDataFromClient.GitHubLink,
            StartDate = projectDataFromClient.StartDate.ToString("MMMM dd"),
            EndDate = projectDataFromClient.EndDate.ToString("MMMM dd"),
            ProjectCode = await UniqueProjectCodeAsync(),
            TeamMemberCount = 1
        };
        _projectRepository.Add(project);

        Project_AppUsers PA = new Project_AppUsers
        {
            ProjectId = project.Id,
            AppUserId = _user.Id,
            Approved = true
        };
        _project_AppUserRepository.Add(PA);

        List<Project> projectList = new List<Project>();
        projectList.Add(project);

        List<ProjCardCtx> projCardCtxList = new List<ProjCardCtx>();
        projCardCtxList.Add(new ProjCardCtx(_user, false));
        List<string[]> randPfpsList = await GetRandProfilePicturesAsync(projectList);
        await Clients.Caller.GetProjCardCtx(projectList, randPfpsList, projCardCtxList);
    }

    public async Task LoadProjects()
    {
        if (_projectRepository.Count() == 0) { return; }

        var myProjects = await _project_AppUserRepository.GetMyProjectsAsync(_user.Id);
        List<ProjCardCtx> projCardCtxList = await GetProjCardCtxList(myProjects);
        List<string[]> randPfpsList = await GetRandProfilePicturesAsync(myProjects);

        await Clients.Caller.GetProjCardCtx(myProjects, randPfpsList, projCardCtxList);
    }

    public async Task SearchProject(string filterString)
    {
        if (_projectRepository.Count() == 0) { return; }

        filterString = filterString.ToUpper();
        var allProjects = await _projectRepository.GetAllAsync();
        var filteredProjects = allProjects.Where(p => p.Name.ToUpper().Contains(filterString)).ToList();

        List<ProjCardCtx> projCardCtxList = await GetProjCardCtxList(filteredProjects);
        List<string[]> randPfpsList = await GetRandProfilePicturesAsync(filteredProjects);

        await Clients.Caller.GetProjCardCtx(filteredProjects, randPfpsList, projCardCtxList);
    }

    public async Task JoinProject(string projectCode)
    {
        var allProjects = await _projectRepository.GetAllAsync();
        var projFromCode = allProjects.Where(p => p.ProjectCode == projectCode).ToList();
        if (projFromCode.Count == 0) {
            await Clients.Caller.JoinProjErr("invalid");
            return;
        }

        var myProjects = await _project_AppUserRepository.GetMyProjectsAsync(_user.Id);
        if (myProjects.Contains(projFromCode[0]))
        {
            await Clients.Caller.JoinProjErr("repeat");
            return;
        }

        List<string[]> randPfpsList = await GetRandProfilePicturesAsync(projFromCode);
        ProjCardCtx projCardCtx = new ProjCardCtx(_user, false, "newProj");
        Project_AppUsers PA = new Project_AppUsers
        {
            ProjectId = projFromCode[0].Id,
            AppUserId = _user.Id,
            Approved = false
        };
        _project_AppUserRepository.Add(PA);
        projFromCode[0].TeamMemberCount++;
        _projectRepository.Update(projFromCode[0]);
        await Clients.Caller.JoinProject(projFromCode[0], randPfpsList[0], projCardCtx);
    }

    public async Task LeaveProject(int projectId)
    {
        var registration = await _project_AppUserRepository.GetByIdAsync(projectId, _user.Id);
        _project_AppUserRepository.Delete(registration);
        var project = await _projectRepository.GetByIdAsync(projectId);
        project.TeamMemberCount--;
        _projectRepository.Update(project);
        if (project.TeamMemberCount == 0)
        {
            _projectRepository.Delete(project);
        }
        await Clients.Caller.DeleteProjCard(projectId);
    }

    private async Task<List<ProjCardCtx>> GetProjCardCtxList(List<Project> projList)
    {
        List<ProjCardCtx> projCardCtxList = new List < ProjCardCtx >();
        foreach (var proj in projList)
        {
            string projCodeResult =
                (await _project_AppUserRepository.IsApproved(proj.Id, _user.Id)) ? "" : "newProj";
            projCardCtxList.Add(new ProjCardCtx(_user, clearBeforeRendering: true, projCodeResult));
        }
        return projCardCtxList;
    }

    private async Task<List<string[]>> GetRandProfilePicturesAsync(List<Project> projList)
    {
        List<string[]> randPfpsList = new List<string[]>();

        Random rnd = new Random();
        for (int i = 0; i < projList.Count; i++)
        {
            List<string> teamMembers =
                await _project_AppUserRepository.GetTeamMembersAsync(projList[i].Id);

            int endIndex = (teamMembers.Count < 3) ? teamMembers.Count : 3;
            string[] randPfps = new string[endIndex];
            List<int> indecesAlreadyUsed = new List<int>();
            for (int j = 0; j < endIndex; j++)
            {
                int rndUserIndex = rnd.Next(teamMembers.Count);
                if (indecesAlreadyUsed.Contains(rndUserIndex)) { j--; continue; }

                randPfps[j] = await _appUserRepo.GetPfpAsync(teamMembers[rndUserIndex]);
                indecesAlreadyUsed.Add(rndUserIndex);
            }
            randPfpsList.Add(randPfps);
        }
        return randPfpsList;
    }

    private async Task<string> UniqueProjectCodeAsync()
    {
        string projectCode = GenerateProjectCode();
        var allProjects = await _projectRepository.GetAllAsync();
        var duplicateProject = allProjects.Where(p => p.ProjectCode == projectCode).ToList();
        if (duplicateProject.Any()) { await UniqueProjectCodeAsync(); }

        return projectCode;
    }
    private string GenerateProjectCode()
    {
        string charArr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random rnd = new Random();

        string projectCode = "";
        for (int i = 0; i < 6; i++)
        {
            int rndIndex = rnd.Next(36);
            projectCode += $"{charArr[rndIndex]} ";
        }

        return projectCode;
    }

    private AppUser GetUser()
    {
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");
        return _appUserRepo.GetById(myId);
    }

    private class ProjectDataFromClient
    {
        public string Name { get; set; }
        public string GitHubLink { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

public class ProjCardCtx
{
    public ProjCardCtx(AppUser appUser, bool clearBeforeRendering, string projCodeResult = "")
    {
        Role = appUser.AssignedRole;
        NotificationCount = appUser.NotificationCount;
        MessageCount = appUser.MessageCount;
        ClearBeforeRendering = clearBeforeRendering;
        ProjCodeResult = projCodeResult;
    }

    public string Role { get; set; }
    public int NotificationCount { get; set; }
    public int MessageCount { get; set; }
    public bool ClearBeforeRendering { get; set; }
    public string TitleId { get; set; } = Guid.NewGuid().ToString();
    public string TopDivId { get; set; } = Guid.NewGuid().ToString();
    public string NotiCountId { get; set; } = Guid.NewGuid().ToString();
    public string MsgCountId { get; set; } = Guid.NewGuid().ToString();
    public string PfpDivId { get; set; } = Guid.NewGuid().ToString();
    public string CopyBtnId { get; set; } = Guid.NewGuid().ToString();
    public string ProjectCodeId { get; set; } = Guid.NewGuid().ToString();
    public string ProjCodeResult { get; set; }
    public string LeaveBtnId { get; set; } = Guid.NewGuid().ToString();
    public string YesDelBtn { get; set; } = Guid.NewGuid().ToString();
    public string OpenBtnId { get; set; } = Guid.NewGuid().ToString();
}
