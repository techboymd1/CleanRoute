using System.Security.Claims;
using CleanRoute.Models;
using CleanRoute.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace CleanRoute.Pages.Requests;

public class IndexModel(RequestService requests) : PageModel
{
    public List<CollectionRequest> Requests { get; private set; } = [];
    public async Task OnGetAsync()
    {
        if (User.IsInRole("Admin"))
            Requests = await requests.GetOpenRequestsAsync();
        else if (User.IsInRole("Collector") && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var collectorId))
            Requests = await requests.GetForCollectorAsync(collectorId);
        else if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var residentId))
            Requests = await requests.GetForResidentAsync(residentId);
    }
}
