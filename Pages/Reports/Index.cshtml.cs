using CleanRoute.Data;
using CleanRoute.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CleanRoute.Pages.Reports;

public class IndexModel(CleanRouteDbContext database) : PageModel
{
    public int TotalRequests { get; private set; }
    public int CollectionRate { get; private set; }
    public int ActiveCollectors { get; private set; }
    public List<ReportItem> ByWasteType { get; private set; } = [];
    public record ReportItem(string Name, int Count, int Percent);
    public async Task OnGetAsync()
    {
        TotalRequests = await database.CollectionRequests.CountAsync();
        var collected = await database.CollectionRequests.CountAsync(request => request.Status == RequestStatus.Collected);
        CollectionRate = TotalRequests == 0 ? 0 : (int)Math.Round(collected * 100d / TotalRequests);
        ActiveCollectors = await database.Users.CountAsync(user => user.Role == UserRole.Collector && user.IsActive);
        var groups = await database.CollectionRequests.GroupBy(request => request.WasteType).Select(group => new { Name = group.Key.ToString(), Count = group.Count() }).OrderByDescending(group => group.Count).ToListAsync();
        ByWasteType = groups.Select(group => new ReportItem(group.Name, group.Count, TotalRequests == 0 ? 0 : (int)Math.Round(group.Count * 100d / TotalRequests))).ToList();
    }
}
