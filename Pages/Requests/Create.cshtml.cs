using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CleanRoute.Models;
using CleanRoute.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CleanRoute.Pages.Requests;

[Authorize(Roles = "Resident")]
public class CreateModel(RequestService requests) : PageModel
{
    [BindProperty] public RequestInput Input { get; set; } = new() { PreferredDate = DateTime.Today.AddDays(1) };
    public class RequestInput
    {
        [Required] public WasteType WasteType { get; set; }
        [Required, StringLength(500)] public string CollectionAddress { get; set; } = string.Empty;
        [Required, DataType(DataType.Date)] public DateTime PreferredDate { get; set; }
        [StringLength(500)] public string? Notes { get; set; }
    }
    public void OnGet() { }
    public async Task<IActionResult> OnPostAsync()
    {
        if (Input.PreferredDate.Date < DateTime.Today) ModelState.AddModelError("Input.PreferredDate", "Choose today or a future date.");
        if (!ModelState.IsValid) return Page();
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var residentId)) return Challenge();
        var request = await requests.CreateAsync(new CollectionRequest { WasteType = Input.WasteType, CollectionAddress = Input.CollectionAddress, PreferredDate = Input.PreferredDate, Notes = Input.Notes }, residentId);
        TempData["Success"] = $"Request #CR-{request.Id:D4} was submitted.";
        return RedirectToPage("Details", new { id = request.Id });
    }
}
