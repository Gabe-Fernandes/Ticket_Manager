using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TicketManager.Interfaces;
using TicketManager.Models;

namespace TicketManager.Hubs;

[Authorize]
public class ChatHub : Hub<IChatHub>
{
    private readonly IProject_AppUsersRepository _paRepo;
    private readonly IMessageRepository _messageRepo;
    private readonly IAppUserRepository _appUserRepo;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly AppUser _user;

    public ChatHub(IMessageRepository messageRepo,
        IAppUserRepository appUserRepo,
        IHttpContextAccessor contextAccessor,
        IProject_AppUsersRepository paRepo)
    {
        _messageRepo = messageRepo;
        _appUserRepo = appUserRepo;
        _contextAccessor = contextAccessor;
        _paRepo = paRepo;
        _user = GetUser();
    }

    public async Task FilterUsers(string filterText, List<string> usersInActiveWindows)
    {
        filterText = filterText.ToUpper();

        var usersFromProject = await _paRepo.GetTeamMembersAsync(_user.CurrentProjectId);
        var filteredUsers = usersFromProject.Where(
            u => (u.FirstName + " " + u.LastName).ToUpper().Contains(filterText)
            && u.Id != _user.Id).ToList();

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

    public async Task LoadMessages(string coworkerId)
    {
        var allMessages = await _messageRepo.GetAllFromProjAsync(_user.CurrentProjectId);
        var ourMessages = allMessages.Where(m =>
            (m.SenderId == coworkerId && m.RecipientId == _user.Id) ||
            (m.SenderId == _user.Id && m.RecipientId == coworkerId)).ToList();
        
        await Clients.Caller.MessagesLoaded(ourMessages, coworkerId);
    }

    public async Task SendMessage(string recipientId, string messageBody)
    {
        var recipient = await _appUserRepo.GetByIdAsync(recipientId);

        Message message = new Message
        {
            SenderName = _user.FirstName + " " + _user.LastName,
            RecipientName = recipient.FirstName + " " + recipient.LastName,
            Body = messageBody,
            Date = DateTime.Now,
            RecipientId = recipientId,
            SenderId = _user.Id,
            ProjectId = _user.CurrentProjectId
        };
        _messageRepo.Add(message);

        ChatGuidList chatGuidList = new ChatGuidList();
        ChatUserContext userCtx = new ChatUserContext(_user);

        await Clients.User(_user.Id).MessageSent(message);
        await Clients.User(recipientId).MessageReceived(chatGuidList, userCtx);
    }

    public async Task GenerateChatGuidList()
    {
        ChatGuidList chatGuidList = new ChatGuidList();
        await Clients.Caller.RenderSearchWindow(chatGuidList);
    }
    
    public async Task OpenChatFromNav(string coworkerId)
    {
        var coworker = await _appUserRepo.GetByIdAsync(coworkerId);
        ChatGuidList chatGuidList = new ChatGuidList();
        ChatUserContext userCtx = new ChatUserContext(coworker);

        await Clients.Caller.MessageReceived(chatGuidList, userCtx);
    }

    private AppUser GetUser()
    {
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");
        return _appUserRepo.GetById(myId);
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
        SelectUserBtnId = Guid.NewGuid().ToString();
    }

    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AssignedRole { get; set; }
    public string ProfilePicture { get; set; }
    public string SelectUserBtnId { get; set; }
}
