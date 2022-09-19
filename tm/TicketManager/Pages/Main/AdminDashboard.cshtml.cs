using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TicketManager.Pages.Main;

[Authorize(Policy = "AdminOnly")]
public class AdminDashboardModel : PageModel {}
