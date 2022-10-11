using TicketManager.Models;

namespace TicketManager.Hubs;

public interface ITleadDashHub
{
    Task GetTickets(List<Ticket> ticketList);
    Task GetTeamMembers(List<TicketMemberCtx> ticketMemberCtxList);
    Task UpdateTicket(Ticket ticket);
    Task DeleteTicket(Ticket ticket);
    Task PostComment(List<Comment> commentList);
}
