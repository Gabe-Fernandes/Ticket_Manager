using TicketManager.Models;

namespace TicketManager.Hubs;

public interface IDevDashHub
{
    Task GetMyTickets(List<Ticket> ticketList);
}
