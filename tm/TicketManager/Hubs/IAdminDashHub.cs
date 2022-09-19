namespace TicketManager.Hubs;

public interface IAdminDashHub
{
    Task GetTeamMembers(List<MemberCtx> memberCtxList);
    Task AddTeamMember(MemberCtx memberCtx);
}
