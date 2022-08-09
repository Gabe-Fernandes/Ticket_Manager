using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TicketManager.Pages.Main;

public class MyProjectsModel : PageModel
{
    private readonly ILogger<MyProjectsModel> _logger;

    public MyProjectsModel(ILogger<MyProjectsModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
