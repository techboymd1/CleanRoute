using System.Security.Claims;
using CleanRoute.Data;
using CleanRoute.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CleanRoute.Pages.Schedules;

public class IndexModel(CleanRouteDbContext database) : PageModel
{
    public List<CollectionSchedule> Schedules { get; private set; } = [];
    public async Task OnGetAsync()
    {
        var query = database.CollectionSchedules.Include(schedule => schedule.CollectionRequest).ThenInclude(request => request.Resident).AsQueryable();
        if (!User.IsInRole("Admin") && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var collectorId)) query = query.Where(schedule => schedule.CollectorId == collectorId);
        Schedules = await query.OrderBy(schedule => schedule.ScheduledFor).ToListAsync();
    }
}
