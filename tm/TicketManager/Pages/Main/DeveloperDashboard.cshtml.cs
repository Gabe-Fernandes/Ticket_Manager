using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TicketManager.Pages.Main;

[Authorize]
public class DeveloperDashboardModel : PageModel
{
    public void OnGet()
    {
    }
}
