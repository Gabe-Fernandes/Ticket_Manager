using TicketManager.Models;

namespace TicketManager.Hubs;

public interface IMyProjectsHub
{
    Task GetProjCardCtx(
        List<Project> projects,
        List<string[]> randPfpsList,
        List<ProjCardCtx> projCardCtxList);
    Task JoinProject(
        Project project,
        string[] randPfps,
        ProjCardCtx projCardCtx);
    Task JoinProjErr(string result);
    Task DeleteProjCard(int projId);
}
