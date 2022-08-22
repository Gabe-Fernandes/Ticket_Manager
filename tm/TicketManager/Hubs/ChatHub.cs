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

    public async Task FilterUsers(string filterText)
    {
        filterText = filterText.ToUpper();
        string myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");

        var UsersFromProject = await _appUserRepo.GetAllAsync(); // Make this the collection of users on this project
        var filteredUsers = UsersFromProject.Where(
            u => (u.FirstName.ToUpper() + " " + u.LastName.ToUpper()).Contains(filterText)
            && u.Id != myId).ToList();

        await Clients.Caller.UsersFiltered(filteredUsers);
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
        var myId = _contextAccessor.HttpContext.User.FindFirstValue("Id");

        Message message = new Message
        {
            Name = _contextAccessor.HttpContext.User.Identity.Name,
            Body = messageBody,
            Date = DateTime.Now,
            From = myId,
            To = recipientId
        };
        _messageRepo.Add(message);

        await Clients.Users(recipientId, myId).MessageSent(message);
    }

    public async Task GenerateChatGuidList()
    {
        ChatGuidList chatGuidList = new ChatGuidList();
        await Clients.Caller.RenderChatWindow(chatGuidList);
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
}
