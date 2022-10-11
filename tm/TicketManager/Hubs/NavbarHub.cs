using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TicketManager.Interfaces;
using TicketManager.Models;

namespace TicketManager.Hubs;

[Authorize]
public class NavbarHub : Hub<INavbarHub>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAppUserRepository _appUserRepository;
    private readonly IMessageRepository _messageRepo;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly AppUser _user;

    public NavbarHub(IMessageRepository messageRepo,
        IHttpContextAccessor contextAccessor,
        ITicketRepository ticketRepository,
        IAppUserRepository appUserRepository)
    {
        _messageRepo = messageRepo;
        _contextAccessor = contextAccessor;
        _ticketRepository = ticketRepository;
        _appUserRepository = appUserRepository;
        _user = GetUser();
    }

    public async Task GetData(string contentId, bool clearContent, int startIndex = 0)
    {
        var dataToDisplay = new List<PanelData>();
        bool hideShowMoreBtn = false;
        const int numOfItemsLoadedCap = 5;
        if (contentId == "chatContent")
        {
            var messagesFromProject = await _messageRepo.GetAllFromProjAsync(_user.CurrentProjectId);
            var myMessages = messagesFromProject.Where(
                m => m.SenderId == _user.Id || m.RecipientId == _user.Id).ToList();

            var usersToDisplay = new List<string>();
            var myConversations = new List<Message>();
            GetMessages(myMessages, usersToDisplay, numOfItemsLoadedCap + startIndex, _user.Id, myConversations);
            hideShowMoreBtn = await PopulateChatPanelData(
                myConversations, usersToDisplay, numOfItemsLoadedCap, dataToDisplay, startIndex);
            await Clients.Caller.PanelDataReceiver(contentId, dataToDisplay, clearContent, hideShowMoreBtn);
        }
        else if (contentId == "notificationContent")
        {
            var ticketsFromProject = await _ticketRepository.GetAllFromProjectAsync(_user.CurrentProjectId);
            var myTickets = ticketsFromProject.Where(t => t.RecipientId == _user.Id).ToList();
            hideShowMoreBtn = PopulateTicketPanelData(
                dataToDisplay, myTickets, numOfItemsLoadedCap, startIndex);
            await Clients.Caller.PanelDataReceiver(contentId, dataToDisplay, clearContent, hideShowMoreBtn);
        }
    }

    public async Task FilterContent(string contentId, string filterString)
    {
        if (string.IsNullOrEmpty(filterString))
        {
            await GetData(contentId, true);
            return;
        }
        filterString = filterString.ToUpper();
        var dataToDisplay = new List<PanelData>();

        if (contentId == "chatContent")
        {
            var messagesFromProject = await _messageRepo.GetAllFromProjAsync(_user.CurrentProjectId);
            var myMessages = messagesFromProject.Where(
                m => (m.RecipientId == _user.Id && m.SenderName.ToUpper().Contains(filterString)) ||
                m.SenderId == _user.Id && m.RecipientName.ToUpper().Contains(filterString)).ToList();

            var usersToDisplay = new List<string>();
            var myConversations = new List<Message>();
            GetMessages(myMessages, usersToDisplay, numOfChatsLoadedCap: 100, _user.Id, myConversations);
            await PopulateChatPanelData(
                myConversations, usersToDisplay, numOfChatsLoadedCap: 100, dataToDisplay);
        }
        else if (contentId == "notificationContent")
        {
            var ticketsFromProject = await _ticketRepository.GetAllFromProjectAsync(_user.CurrentProjectId);
            var myTickets = ticketsFromProject.Where(
                t => t.RecipientId == _user.Id && t.Description.ToUpper().Contains(filterString)).ToList();
            PopulateTicketPanelData(dataToDisplay, myTickets, numOfItemsLoadedCap: 100);
        }

        await Clients.Caller.PanelDataReceiver(
            contentId, dataToDisplay, clearContent: true, hideShowMoreBtn: true);
    }

    private void GetMessages(
        List<Message> myMessages, List<string> usersToDisplay,
        int numOfChatsLoadedCap, string myId, List<Message> myConversations)
    {
        for (int i = myMessages.Count() - 1; i >= 0; i--)
        {
            bool redundantMsg = false;
            foreach (var id in usersToDisplay)
            {
                if (myMessages[i].SenderId == id || myMessages[i].RecipientId == id)
                {
                    redundantMsg = true;
                    break;
                }
            }
            if (redundantMsg) { continue; }
            myConversations.Add(myMessages[i]);

            string newUserId = 
                (myMessages[i].SenderId == myId) ? myMessages[i].RecipientId : myMessages[i].SenderId;
            usersToDisplay.Add(newUserId);

            if (usersToDisplay.Count == numOfChatsLoadedCap) { break; }
        }
    }

    private async Task<bool> PopulateChatPanelData(
        List<Message> myConversations,
        List<string> usersToDisplay,
        int numOfChatsLoadedCap,
        List<PanelData> dataToDisplay,
        int startIndex = 0)
    {
        int endIndex = (usersToDisplay.Count < numOfChatsLoadedCap) ?
            usersToDisplay.Count : numOfChatsLoadedCap;
        endIndex += startIndex;

        for (int i = startIndex; i < endIndex; i++)
        {
            if (i == usersToDisplay.Count) { return true; }
            var coworker = await _appUserRepository.GetByIdAsync(usersToDisplay[i]);

            PanelData data = new PanelData
            {
                coworkerId = coworker.Id,
                Title = coworker.FirstName + " " + coworker.LastName,
                ImgSrc = coworker.ProfilePicture,
                Description = myConversations[i].Body,
                Time = myConversations[i].Date.ToString("MM/dd/yy - hh:mm tt"),
                SelectNavMenuOptionId = Guid.NewGuid().ToString()
            };
            dataToDisplay.Add(data);
        }
        return false;
    }

    private bool PopulateTicketPanelData(List<PanelData> dataToDisplay, List<Ticket> myTickets,
        int numOfItemsLoadedCap, int startIndex = 0)
    {
        int endIndex = (myTickets.Count < numOfItemsLoadedCap) ?
            myTickets.Count : numOfItemsLoadedCap;
        endIndex += startIndex;

        for (int i = startIndex; i < endIndex; i++)
        {
            if (i == myTickets.Count) { return true; }

            PanelData data = new PanelData
            {
                coworkerId = myTickets[i].SenderId,
                Title = myTickets[i].SenderName,
                ImgSrc = "/icons/ticketIcon.png",
                Description = myTickets[i].Description,
                Time = myTickets[i].EndDate,
                SelectNavMenuOptionId = Guid.NewGuid().ToString()
            };
            dataToDisplay.Add(data);
        }
        return false;
    }

    private AppUser GetUser()
    {
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");
        return _appUserRepository.GetById(myId);
    }
}

public class PanelData
{
    public string coworkerId { get; set; }
    public string Title { get; set; }
    public string ImgSrc { get; set; }
    public string Description { get; set; }
    public string Time { get; set; }
    public string SelectNavMenuOptionId { get; set; }
}
