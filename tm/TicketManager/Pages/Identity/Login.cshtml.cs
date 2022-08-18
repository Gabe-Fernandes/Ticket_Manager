using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManager.Models;

namespace TicketManager.Pages.Identity;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;
    private readonly UserManager<AppUser> _userManager;

    public LoginModel(SignInManager<AppUser> signInManager,
        ILogger<LoginModel> logger,
        UserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _logger = logger;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    public static string Cookie = "cookie";
    public static string Admin = "admin";
    public static string TechLead = "techLead";
    public static string Developer = "developer";

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync()
    {
        // Clear the existing cookies to ensure a clean login process
        await _signInManager.SignOutAsync();
        await HttpContext.SignOutAsync(Cookie);
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                await GenerateSecurityContextAsync(Input.Email, HttpContext);
                return RedirectToPage("/Main/MyProjects", new { code = "normal_login" });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }

        return Page(); // If we got this far, something failed, redisplay form
    }

    public async Task GenerateSecurityContextAsync(string email, HttpContext context)
    {
        var userFromDb = await _userManager.FindByNameAsync(email);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userFromDb.FirstName),
            new Claim("LastName", userFromDb.LastName),
            new Claim("Email", userFromDb.Email),
            new Claim("Id", userFromDb.Id),
            new Claim(ClaimTypes.Role, userFromDb.AssignedRole),
            new Claim(ClaimTypes.NameIdentifier, userFromDb.Id)
        };

        var identity = new ClaimsIdentity(claims, Cookie);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        await context.SignInAsync(Cookie, principal);

        _logger.LogInformation("User logged in.");
    }
}
