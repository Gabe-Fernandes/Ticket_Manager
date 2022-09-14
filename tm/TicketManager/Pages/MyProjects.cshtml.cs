using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using TicketManager.Models;
using TicketManager.Pages.Identity;

namespace TicketManager.Pages;

public class MyProjectsModel : PageModel
{
    private readonly ILogger<LoginModel> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly LoginModel _loginModel;
    private readonly SignInManager<AppUser> _signInManager;

    public MyProjectsModel(ILogger<LoginModel> logger,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _loginModel = new LoginModel(_signInManager, _logger, _userManager);
    }

    public async Task<IActionResult> OnGet(string code, string userId = null)
    {
        if (code == null)
        {
            return RedirectToPage("/Identity/Login");
        }

        if (User.Identity.IsAuthenticated && (code == "normal_login" || code == "reset_password_login"))
        {
            return Page();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            await _loginModel.GenerateSecurityContextAsync(user.Email, HttpContext);
            TempData["Confirmation"] = "Thank you for confirming your email.";
            return Page();
        }
        else
        {
            TempData["Confirmation"] = "Error confirming your email.";
            return RedirectToPage("/Identity/Login");
        }
    }
}
