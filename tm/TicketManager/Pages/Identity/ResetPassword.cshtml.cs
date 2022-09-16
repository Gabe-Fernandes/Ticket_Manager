using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using TicketManager.Models;

namespace TicketManager.Pages.Identity;

[AllowAnonymous]
public class ResetPasswordModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly LoginModel _loginModel;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public ResetPasswordModel(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ILogger<LoginModel> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _loginModel = new LoginModel(_signInManager, _logger, _userManager);
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Code { get; set; }
    }

    public IActionResult OnGet(string code = null)
    {
        //if (code == null)
        //{
        //    return BadRequest("A code must be supplied for password reset.");
        //}
        //else
        //{
            //Input = new InputModel
            //{
            //    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
            //};
            return Page();
        //}
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            // Add temporary data - reveal if wrong?
            return Page();
        }

        var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (result.Succeeded)
        {
            // Add temporary data
            await _signInManager.PasswordSignInAsync(
                user.Email, Input.Password, isPersistent: false, lockoutOnFailure: false);
            await _loginModel.GenerateSecurityContextAsync(Input.Email, HttpContext);
            return RedirectToPage("/MyProjects", new { code = "reset_password_login" });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return Page();
    }
}
