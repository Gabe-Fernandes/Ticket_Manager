using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TicketManager.Interfaces;
using TicketManager.Models;
using static System.Net.Mime.MediaTypeNames;

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

    public async Task GetData(string contentId, bool clearContent, bool calledFromSearchMethod)
    {
        var dataToDisplay = new List<PanelData>();
        if (contentId == "chatContent")
        {
            const int numOfChatsLoadedCap = 5;
            string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");
            var messagesFromProject = await _messageRepo.GetAllAsync();
            var myMessages = messagesFromProject.Where(
                m => m.To == myId || m.From == myId).ToList();

            var test = new List<string>();
            var myConversations = new List<Message>();
            GetMessages(myMessages, test, numOfChatsLoadedCap, myId, myConversations);
            await PopulatePanelData(myConversations, test, numOfChatsLoadedCap, dataToDisplay);
        }

        await Clients.Caller.PanelDataReceiver(contentId, dataToDisplay, clearContent, calledFromSearchMethod);
    }

    public async Task FilterContent(string contentId, string filterString)
    {
        if (string.IsNullOrEmpty(filterString))
        {
            await GetData(contentId, true, true);
            return;
        }

        filterString = filterString.ToUpper();
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");

        var messagesFromProject = await _messageRepo.GetAllAsync();
        var myMessages = messagesFromProject.Where(
            m => (m.To == myId && m.SenderName.ToUpper().Contains(filterString)) ||
            m.From == myId && m.ReceiverName.ToUpper().Contains(filterString)).ToList();

        var test = new List<string>();
        var myConversations = new List<Message>();
        GetMessages(myMessages, test, 100, myId, myConversations);
        var dataToDisplay = new List<PanelData>();
        await PopulatePanelData(myConversations, test, 100, dataToDisplay);


        await Clients.Caller.PanelDataReceiver(contentId, dataToDisplay, true, false);
    }

    private void GetMessages(
        List<Message> myMessages, List<string> test,
        int numOfChatsLoadedCap, string myId, List<Message> myConversations)
    {
        for (int i = myMessages.Count() - 1; i >= 0; i--)
        {
            bool redundantMsg = false;
            foreach (var id in test)
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
            test.Add(newUserId);

            if (test.Count == numOfChatsLoadedCap) { break; }
        }
    }

    private async Task PopulatePanelData(List<Message> myConversations, List<string> test,
        int numOfChatsLoadedCap, List<PanelData> dataToDisplay)
    {
        int numOfLoops = (test.Count < numOfChatsLoadedCap) ? test.Count : numOfChatsLoadedCap;
        for (int i = 0; i < numOfLoops; i++)
        {
            var coworker = await _appUserRepository.GetByIdAsync(test[i]);

            PanelData data = new PanelData
            {
                Title = coworker.FirstName + " " + coworker.LastName,
                ImgSrc = coworker.ProfilePicture,
                Description = myConversations[i].Body,
                Time = myConversations[i].Date.ToString("MM/dd/yy - hh:mm tt")
            };
            dataToDisplay.Add(data);
        }
    }
}

public class PanelData
{
    public string Title { get; set; }
    public string ImgSrc { get; set; }
    public string Description { get; set; }
    public string Time { get; set; }
}
