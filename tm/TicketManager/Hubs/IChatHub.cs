using TicketManager.Models;

namespace TicketManager.Hubs;

public interface IChatHub
{
    Task UsersFiltered(IEnumerable<AppUser> filteredUsers);
    Task MessagesLoaded(IEnumerable<Message> messages);
    Task MessageSent(Message message);
}
