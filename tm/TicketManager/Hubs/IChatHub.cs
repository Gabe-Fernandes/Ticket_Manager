using TicketManager.Models;

namespace TicketManager.Hubs;

public interface IChatHub
{
    Task UsersFiltered(IEnumerable<ChatUserContext> chatUserContextList);
    Task MessagesLoaded(IEnumerable<Message> messages, string coworkerId);
    Task MessageSent(Message message);
    Task MessageReceived(ChatGuidList chatGuidList, ChatUserContext chatUserContext);
    Task RenderSearchWindow(ChatGuidList chatWindowIds);
}
