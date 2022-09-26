using TicketManager.Models;

namespace TicketManager.Hubs;

public interface ITleadDashHub
{
    Task GetTickets(List<Ticket> ticketList, string detailsBtnId);
    Task GetTeamMembers(List<TicketMemberCtx> ticketMemberCtxList);
}
