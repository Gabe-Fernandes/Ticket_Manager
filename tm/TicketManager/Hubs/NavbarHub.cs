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

    public NavbarHub(IMessageRepository messageRepo,
        IHttpContextAccessor contextAccessor,
        ITicketRepository ticketRepository,
        IAppUserRepository appUserRepository)
    {
        _messageRepo = messageRepo;
        _contextAccessor = contextAccessor;
        _ticketRepository = ticketRepository;
        _appUserRepository = appUserRepository;
    }

    public async Task GetData(string contentId, bool clearContent, int startIndex = 0)
    {
        var dataToDisplay = new List<PanelData>();
        bool hideShowMoreBtn = false;
        if (contentId == "chatContent")
        {
            const int numOfChatsLoadedCap = 5;
            string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");
            var messagesFromProject = await _messageRepo.GetAllAsync();
            var myMessages = messagesFromProject.Where(
                m => m.To == myId || m.From == myId).ToList();

            var usersToDisplay = new List<string>();
            var myConversations = new List<Message>();
            GetMessages(myMessages, usersToDisplay, numOfChatsLoadedCap + startIndex, myId, myConversations);
            hideShowMoreBtn = await PopulatePanelData(
                myConversations, usersToDisplay, numOfChatsLoadedCap, dataToDisplay, startIndex);
        }

        await Clients.Caller.PanelDataReceiver(contentId, dataToDisplay, clearContent, hideShowMoreBtn);
    }

    public async Task FilterContent(string contentId, string filterString)
    {
        if (string.IsNullOrEmpty(filterString))
        {
            await GetData(contentId, true);
            return;
        }

        filterString = filterString.ToUpper();
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");

        var messagesFromProject = await _messageRepo.GetAllAsync();
        var myMessages = messagesFromProject.Where(
            m => (m.To == myId && m.SenderName.ToUpper().Contains(filterString)) ||
            m.From == myId && m.ReceiverName.ToUpper().Contains(filterString)).ToList();

        var usersToDisplay = new List<string>();
        var myConversations = new List<Message>();
        GetMessages(myMessages, usersToDisplay, 100, myId, myConversations);
        var dataToDisplay = new List<PanelData>();
        await PopulatePanelData(myConversations, usersToDisplay, 100, dataToDisplay);

        await Clients.Caller.PanelDataReceiver(contentId, dataToDisplay, true, true);
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
                if (myMessages[i].To == id || myMessages[i].From == id)
                {
                    redundantMsg = true;
                    break;
                }
            }
            if (redundantMsg) { continue; }
            myConversations.Add(myMessages[i]);

            string newUserId = (myMessages[i].To == myId) ? myMessages[i].From : myMessages[i].To;
            usersToDisplay.Add(newUserId);

            if (usersToDisplay.Count == numOfChatsLoadedCap) { break; }
        }
    }

    private async Task<bool> PopulatePanelData(List<Message> myConversations, List<string> usersToDisplay,
        int numOfChatsLoadedCap, List<PanelData> dataToDisplay, int startIndex = 0)
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
