using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TicketManager.Pages.Main;

[Authorize(Policy = "Management")]
public class TechLeadDashboardModel : PageModel
{
    public void OnGet()
    {

    }
}
