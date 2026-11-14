using CleanRoute.Data;
using CleanRoute.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CleanRoute.Pages;

public class IndexModel(CleanRouteDbContext database) : PageModel
{
    public int PendingRequests { get; private set; }
    public int TodaySchedules { get; private set; }
    public int CompletedThisMonth { get; private set; }
    public List<CollectionRequest> UpcomingRequests { get; private set; } = [];
    public async Task OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated != true) return;
        var today = DateTime.Today; var monthStart = new DateTime(today.Year, today.Month, 1);
        var currentUserId = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var parsedId) ? parsedId : 0;
        var requests = database.CollectionRequests.Include(request => request.Resident).Include(request => request.Schedule).AsQueryable();
        var schedules = database.CollectionSchedules.AsQueryable();
        if (User.IsInRole("Resident")) { requests = requests.Where(request => request.ResidentId == currentUserId); schedules = schedules.Where(schedule => schedule.CollectionRequest.ResidentId == currentUserId); }
        else if (User.IsInRole("Collector")) { requests = requests.Where(request => request.Schedule != null && request.Schedule.CollectorId == currentUserId); schedules = schedules.Where(schedule => schedule.CollectorId == currentUserId); }
        PendingRequests = await requests.CountAsync(request => request.Status == RequestStatus.Submitted);
        TodaySchedules = await schedules.CountAsync(schedule => schedule.ScheduledFor.Date == today);
        CompletedThisMonth = await schedules.CountAsync(schedule => schedule.CompletedAtUtc >= monthStart);
        UpcomingRequests = await requests.Where(request => request.Status != RequestStatus.Cancelled).OrderBy(request => request.PreferredDate).Take(5).ToListAsync();
    }
}
