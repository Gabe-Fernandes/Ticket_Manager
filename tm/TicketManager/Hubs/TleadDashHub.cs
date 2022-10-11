using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TicketManager.Interfaces;
using TicketManager.Models;

namespace TicketManager.Hubs;

[Authorize]
public class TleadDashHub : Hub<ITleadDashHub>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IProject_AppUsersRepository _paRepo;
    private readonly IAppUserRepository _appUserRepo;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly AppUser _user;

    public TleadDashHub(IProject_AppUsersRepository paRepo,
        IHttpContextAccessor contextAccessor,
        IAppUserRepository appUserRepo,
        ITicketRepository ticketRepository,
        ICommentRepository commentRepository)
    {
        _paRepo = paRepo;
        _contextAccessor = contextAccessor;
        _appUserRepo = appUserRepo;
        _user = GetUser();
        _ticketRepository = ticketRepository;
        _commentRepository = commentRepository;
    }

    public async Task LoadTickets(string filterString)
    {
        List<Ticket> ticketList = await _paRepo.GetTicketsFromProjAsync(_user.CurrentProjectId);
        if (string.IsNullOrEmpty(filterString))
        {
            await Clients.Caller.GetTickets(ticketList);
            return;
        }
        var filteredTickets = ticketList.Where(t => t.Title.ToUpper() == filterString.ToUpper()).ToList();
        await Clients.Caller.GetTickets(filteredTickets);
    }

    public async Task LoadTeamMembers(string modalType)
    {
        List<TicketMemberCtx> ticketMemberCtxList = new List<TicketMemberCtx>();
        List<AppUser> teamMembers = await _paRepo.GetTeamMembersAsync(_user.CurrentProjectId);
        foreach (AppUser teamMember in teamMembers)
        {
            TicketMemberCtx ticketMemberCtx = new TicketMemberCtx(teamMember, modalType);
            ticketMemberCtxList.Add(ticketMemberCtx);
        }
        await Clients.Caller.GetTeamMembers(ticketMemberCtxList);
    }

    public async Task CreateTicket(object formData)
    {
        string formDataString = formData.ToString();
        Ticket ticket = JsonConvert.DeserializeObject<Ticket>(formDataString);
        var recipient = await _appUserRepo.GetByIdAsync(ticket.RecipientId);
        ticket.RecipientName = $"{recipient.FirstName} {recipient.LastName}";
        ticket.RecipientPfp = recipient.ProfilePicture;
        ticket.StartDate = DateTime.Now.ToString("MM/dd");
        ticket.EndDate = ticket.TempDate.ToString("MM/dd");
        ticket.SenderId = _user.Id;
        ticket.SenderName = $"{_user.FirstName} {_user.LastName}";
        ticket.SenderPfp = _user.ProfilePicture;
        ticket.TableRowId = Guid.NewGuid().ToString();
        ticket.DetailsBtnId = Guid.NewGuid().ToString();
        ticket.ProjectId = _user.CurrentProjectId;
        _ticketRepository.Add(ticket);

        List<Ticket> ticketList = new List<Ticket> { ticket };
        List<string> managers = await _paRepo.GetManagers(_user.CurrentProjectId);

        await Clients.Users(managers).GetTickets(ticketList);
    }

    public async Task UpdateTicket(object formData, int ticketId)
    {
        string formDataString = formData.ToString();
        Ticket tempTicket = JsonConvert.DeserializeObject<Ticket>(formDataString);
        Ticket ticket = await _ticketRepository.GetByIdAsync(ticketId);
        ticket.Title = tempTicket.Title;
        ticket.Description = tempTicket.Description;
        ticket.Status = tempTicket.Status;
        ticket.PriorityLevel = tempTicket.PriorityLevel;
        ticket.EndDate = tempTicket.TempDate.ToString("MM/dd");
        ticket.RecipientId = tempTicket.RecipientId;
        var recipient = await _appUserRepo.GetByIdAsync(ticket.RecipientId);
        ticket.RecipientName = recipient.FirstName;
        ticket.RecipientPfp = recipient.ProfilePicture;
        _ticketRepository.Update(ticket);

        await Clients.All.UpdateTicket(ticket);
    }

    public async Task DeleteTicket(int ticketId)
    {
        Ticket ticket = await _ticketRepository.GetByIdAsync(ticketId);
        _ticketRepository.Delete(ticket);
        await Clients.All.DeleteTicket(ticket);
    }

    public async Task ApproveTicket(int ticketId)
    {
        Ticket ticket = await _ticketRepository.GetByIdAsync(ticketId);
        ticket.Status = "Resolved";
        _ticketRepository.Update(ticket);
        await Clients.All.UpdateTicket(ticket);
    }

    public async Task LoadComments(int ticketId)
    {
        List<Comment> comments = await _commentRepository.GetAllAsync(ticketId);
        await Clients.Caller.PostComment(comments);
    }

    public async Task PostComment(string commentBody, int ticketId, string tableRowId)
    {
        Comment comment = new Comment
        {
            FirstName = _user.FirstName,
            LastName = _user.LastName,
            Pfp = _user.ProfilePicture,
            Body = commentBody,
            Date = DateTime.Now.ToString("MM/dd - hh:mm tt"),
            TicketId = ticketId,
            TableRowId = tableRowId
        };
        _commentRepository.Add(comment);
        List<Comment> commentList = new List<Comment> { comment };

        await Clients.All.PostComment(commentList);
    }

    private AppUser GetUser()
    {
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");
        return _appUserRepo.GetById(myId);
    }
}

public class TicketMemberCtx
{
    public TicketMemberCtx(AppUser appUser, string modalType)
    {
        RecipientId = appUser.Id;
        Pfp = appUser.ProfilePicture;
        FirstName = appUser.FirstName;
        LastName = appUser.LastName;
        ModalType = modalType;
    }
    
    public string UserDivId { get; } = Guid.NewGuid().ToString();
    public string userRadioBtnId { get; } = Guid.NewGuid().ToString();
    public string RecipientId { get; }
    public string Pfp { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string ModalType { get; }
}
