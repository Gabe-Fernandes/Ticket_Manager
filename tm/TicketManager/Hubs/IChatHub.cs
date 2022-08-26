using TicketManager.Models;

namespace TicketManager.Hubs;

public interface IChatHub
{
    Task UsersFiltered(IEnumerable<ChatUserContext> chatUserContextList);
    Task MessagesLoaded(IEnumerable<Message> messages);
    Task MessageSent(Message message);
    Task RenderNewChatWindow(ChatGuidList chatWindowIds);
}
