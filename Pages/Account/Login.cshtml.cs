using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CleanRoute.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CleanRoute.Pages.Account;

public class LoginModel(AccountService accounts) : PageModel
{
    [BindProperty] public LoginInput Input { get; set; } = new();
    public class LoginInput
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }
    public void OnGet() { }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var user = await accounts.ValidateCredentialsAsync(Input.Email, Input.Password);
        if (user is null) { ModelState.AddModelError(string.Empty, "Email or password is incorrect."); return Page(); }
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.FullName), new Claim(ClaimTypes.Role, user.Role.ToString()) };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        return LocalRedirect(Url.Content("~/"));
    }
}
