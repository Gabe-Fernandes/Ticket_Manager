using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TicketManager.Interfaces;
using TicketManager.Models;

namespace TicketManager.Hubs;

[Authorize]
public class ChatHub : Hub<IChatHub>
{
    private readonly IMessageRepository _messageRepo;
    private readonly IAppUserRepository _appUserRepo;
    private readonly IHttpContextAccessor _contextAccessor;

    public ChatHub(IMessageRepository messageRepo,
        IAppUserRepository appUserRepo,
        IHttpContextAccessor contextAccessor)
    {
        _messageRepo = messageRepo;
        _appUserRepo = appUserRepo;
        _contextAccessor = contextAccessor;
    }

    public async Task FilterUsers(string filterText, List<string> usersInActiveWindows)
    {
        filterText = filterText.ToUpper();
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");

        var UsersFromProject = await _appUserRepo.GetAllAsync(); // Make this the collection of users on this project
        var filteredUsers = UsersFromProject.Where(
            u => (u.FirstName + " " + u.LastName).ToUpper().Contains(filterText)
            && u.Id != myId).ToList();

        // we don't want to send sensitive data from appuser to the client
        var userCtxList = new List<ChatUserContext>();
        foreach (var user in filteredUsers)
        {
            if (usersInActiveWindows.Contains(user.Id) == false)
            {
                ChatUserContext userCtx = new ChatUserContext(user);
                userCtxList.Add(userCtx);
            }
        }

        await Clients.Caller.UsersFiltered(userCtxList);
    }

    public async Task LoadMessages(string coworkerId) // algo could improve
    {
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");

        var allMessages = await _messageRepo.GetAllAsync(); //Limit to project 
        var ourMessages = allMessages.Where(m =>
            (m.To == coworkerId && m.From == myId) ||
            (m.To == myId && m.From == coworkerId)).ToList();
        
        await Clients.Caller.MessagesLoaded(ourMessages);
    }

    public async Task SendMessage(string recipientId, string messageBody)
    {
        var senderId = _contextAccessor.HttpContext.User.FindFirstValue("Id");
        var sender = await _appUserRepo.GetByIdAsync(senderId);
        var recipient = await _appUserRepo.GetByIdAsync(recipientId);

        Message message = new Message
        {
            SenderName = sender.FirstName + " " + sender.LastName,
            ReceiverName = recipient.FirstName + " " + recipient.LastName,
            Body = messageBody,
            Date = DateTime.Now,
            From = senderId,
            To = recipientId
        };
        _messageRepo.Add(message);

        ChatGuidList chatGuidList = new ChatGuidList();
        ChatUserContext userCtx = new ChatUserContext(sender);

        await Clients.User(senderId).MessageSent(message);
        await Clients.User(recipientId).MessageReceived(chatGuidList, userCtx);
    }

    public async Task GenerateChatGuidList()
    {
        ChatGuidList chatGuidList = new ChatGuidList();
        await Clients.Caller.RenderSearchWindow(chatGuidList);
    }
}

public class ChatGuidList
{
    public string ChatWindowId { get; } = Guid.NewGuid().ToString();
    public string PfpId { get; } = Guid.NewGuid().ToString();
    public string NameTagId { get; } = Guid.NewGuid().ToString();
    public string SearchBoxId { get; } = Guid.NewGuid().ToString();
    public string MinimizeBtnId { get; } = Guid.NewGuid().ToString();
    public string CloseBtnId { get; } = Guid.NewGuid().ToString();
    public string ContentListId { get; } = Guid.NewGuid().ToString();
    public string MessageInputId { get; } = Guid.NewGuid().ToString();
    public string SendBtnId { get; } = Guid.NewGuid().ToString();
    public string MidId { get; } = Guid.NewGuid().ToString();
    public string BotId { get; } = Guid.NewGuid().ToString();
    public string MinimizedChatWindowId { get; } = Guid.NewGuid().ToString();
}

public class ChatUserContext
{
    public ChatUserContext(AppUser appUser)
    {
        Id = appUser.Id;
        FirstName = appUser.FirstName;
        LastName = appUser.LastName;
        AssignedRole = appUser.AssignedRole;
        ProfilePicture = appUser.ProfilePicture;
    }

    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AssignedRole { get; set; }
    public string ProfilePicture { get; set; }
}
