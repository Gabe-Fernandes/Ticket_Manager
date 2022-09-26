using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TicketManager.Interfaces;
using TicketManager.Models;
using TicketManager.Pages.Identity;

namespace TicketManager.Hubs;

public class TleadDashHub : Hub<ITleadDashHub>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IProject_AppUsersRepository _paRepo;
    private readonly IAppUserRepository _appUserRepo;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly AppUser _user;

    public TleadDashHub(IProject_AppUsersRepository paRepo,
        IHttpContextAccessor contextAccessor,
        IAppUserRepository appUserRepo,
        ITicketRepository ticketRepository)
    {
        _paRepo = paRepo;
        _contextAccessor = contextAccessor;
        _appUserRepo = appUserRepo;
        _user = GetUser();
        _ticketRepository = ticketRepository;
    }

    public async Task LoadTickets(string filterString)
    {
        List<Ticket> ticketList = await _paRepo.GetTicketsFromProjAsync(_user.CurrentProjectId);
        string detailsBtnId = Guid.NewGuid().ToString();
        if (string.IsNullOrEmpty(filterString))
        {
            await Clients.Caller.GetTickets(ticketList, detailsBtnId);
            return;
        }
        var filteredTickets = ticketList.Where(t => t.Title.ToUpper() == filterString.ToUpper()).ToList();
        await Clients.Caller.GetTickets(filteredTickets, detailsBtnId);
    }

    public async Task LoadTeamMembers()
    {
        List<TicketMemberCtx> ticketMemberCtxList = new List<TicketMemberCtx>();
        List<AppUser> teamMembers = await _paRepo.GetTeamMembersAsync(_user.CurrentProjectId);
        foreach (AppUser teamMember in teamMembers)
        {
            TicketMemberCtx ticketMemberCtx = new TicketMemberCtx(teamMember);
            ticketMemberCtxList.Add(ticketMemberCtx);
        }
        await Clients.Caller.GetTeamMembers(ticketMemberCtxList);
    }

    public async Task CreateTicket(object formData)
    {
        string formDataString = formData.ToString();
        Ticket ticket = JsonConvert.DeserializeObject<Ticket>(formDataString);
        var recipient = await _appUserRepo.GetByIdAsync(ticket.RecipientId);
        ticket.RecipientName = recipient.FirstName;
        ticket.StartDate = DateTime.Now.ToString("MM/dd/yy");
        ticket.EndDate = ticket.TempDate.ToString("MM/dd/yy");
        ticket.SenderId = _user.Id;
        ticket.SenderName = _user.FirstName;
        ticket.ProjectId = _user.CurrentProjectId;
        _ticketRepository.Add(ticket);

        List<Ticket> ticketList = new List<Ticket> { ticket };

        List<string> admins = await _paRepo.GetIdsOfRoleAsync(_user.CurrentProjectId, LoginModel.Admin);
        List<string> techLeads = await _paRepo.GetIdsOfRoleAsync(_user.CurrentProjectId, LoginModel.TechLead);
        List<string> managers = admins.Concat(techLeads).ToList();
        string detailsBtnId = Guid.NewGuid().ToString();

        await Clients.Users(managers).GetTickets(ticketList, detailsBtnId);
    }

    private AppUser GetUser()
    {
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");
        return _appUserRepo.GetById(myId);
    }
}

public class TicketMemberCtx
{
    public TicketMemberCtx(AppUser appUser)
    {
        RecipientId = appUser.Id;
        Pfp = appUser.ProfilePicture;
        FirstName = appUser.FirstName;
        LastName = appUser.LastName;
    }
    
    public string UserDivId { get; } = Guid.NewGuid().ToString();
    public string userRadioBtnId { get; } = Guid.NewGuid().ToString();
    public string RecipientId { get; }
    public string Pfp { get; }
    public string FirstName { get; }
    public string LastName { get; }
}
