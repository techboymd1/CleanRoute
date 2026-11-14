using System.ComponentModel.DataAnnotations;
using CleanRoute.Models;
using CleanRoute.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CleanRoute.Pages.Account;

public class RegisterModel(AccountService accounts) : PageModel
{
    [BindProperty] public RegistrationInput Input { get; set; } = new();
    public class RegistrationInput
    {
        [Required, StringLength(100)] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(150)] public string Email { get; set; } = string.Empty;
        [Required, StringLength(25)] public string PhoneNumber { get; set; } = string.Empty;
        [Required, StringLength(200)] public string Address { get; set; } = string.Empty;
        [Required, MinLength(8), DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        [Required, DataType(DataType.Password), Compare(nameof(Password))] public string ConfirmPassword { get; set; } = string.Empty;
    }
    public void OnGet() { }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var result = await accounts.RegisterResidentAsync(new ApplicationUser { FullName = Input.FullName, Email = Input.Email, PhoneNumber = Input.PhoneNumber, Address = Input.Address }, Input.Password);
        if (!result.Succeeded) { ModelState.AddModelError(string.Empty, result.Error!); return Page(); }
        TempData["Success"] = "Account created. You can now sign in.";
        return RedirectToPage("Login");
    }
}
