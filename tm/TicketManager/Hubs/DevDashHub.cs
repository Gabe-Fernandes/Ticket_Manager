using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using TicketManager.Interfaces;
using TicketManager.Models;

namespace TicketManager.Hubs;

public class DevDashHub : Hub<IDevDashHub>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IProject_AppUsersRepository _paRepo;
    private readonly IAppUserRepository _appUserRepo;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly AppUser _user;

    public DevDashHub(IProject_AppUsersRepository paRepo,
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

    public async Task LoadMyTickets(string filterString)
    {
        List<Ticket> ticketList = await _paRepo.GetTicketsFromProjAsync(_user.CurrentProjectId);
        if (string.IsNullOrEmpty(filterString))
        {
            var myTickets = ticketList.Where(t => t.RecipientId == _user.Id).ToList();
            await Clients.Caller.GetMyTickets(myTickets);
            return;
        }
        var filteredTickets = ticketList.Where(t => t.Title.ToUpper().Contains(filterString.ToUpper()) &&
            t.RecipientId == _user.Id).ToList();
        await Clients.Caller.GetMyTickets(filteredTickets);
    }

    public async Task SubmitOrReopenTicket(int ticketId, string status)
    {
        Ticket ticketToUpdate = await _ticketRepository.GetByIdAsync(ticketId);
        ticketToUpdate.Status = status;
        _ticketRepository.Update(ticketToUpdate);
       await LoadMyTickets(string.Empty);
    }

    private AppUser GetUser()
    {
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");
        return _appUserRepo.GetById(myId);
    }
}
