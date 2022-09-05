namespace TicketManager.Hubs;

public interface INavbarHub
{
    Task PanelDataReceiver(
        string contentId,
        IEnumerable<PanelData> panelData,
        bool clearContent,
        bool hideShowMoreBtn);
}
