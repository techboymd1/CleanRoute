using System.Security.Claims;
using CleanRoute.Data;
using CleanRoute.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace CleanRoute.Pages.Requests;

public class DetailsModel(CleanRouteDbContext database) : PageModel
{
    public CollectionRequest CollectionRequest { get; private set; } = null!;
    public List<SelectListItem> CollectorOptions { get; private set; } = [];
    public bool CanSchedule => User.IsInRole("Admin");
    public bool CanComplete => User.IsInRole("Admin") || (CollectionRequest.Schedule?.CollectorId.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier));
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var request = await LoadRequestAsync(id); if (request is null) return NotFound();
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAssignedCollector = request.Schedule?.CollectorId.ToString() == currentUserId;
        if (!CanSchedule && !isAssignedCollector && request.ResidentId.ToString() != currentUserId) return Forbid();
        CollectionRequest = request; await LoadCollectorsAsync(); return Page();
    }
    public async Task<IActionResult> OnPostScheduleAsync(int id, int collectorId, DateTime scheduledFor, string? routeArea)
    {
        if (!CanSchedule) return Forbid();
        var request = await database.CollectionRequests.Include(item => item.Schedule).SingleOrDefaultAsync(item => item.Id == id);
        if (request is null) return NotFound();
        if (request.Status != RequestStatus.Submitted || request.Schedule is not null)
        {
            TempData["Error"] = "Only an unscheduled request can be assigned.";
            return RedirectToPage(new { id });
        }
        if (scheduledFor < DateTime.Now)
        {
            TempData["Error"] = "Choose a future collection date and time.";
            return RedirectToPage(new { id });
        }
        var collectorExists = await database.Users.AnyAsync(user => user.Id == collectorId && user.Role == UserRole.Collector && user.IsActive);
        if (!collectorExists)
        {
            TempData["Error"] = "Choose an active collector.";
            return RedirectToPage(new { id });
        }
        database.CollectionSchedules.Add(new CollectionSchedule { CollectionRequestId = id, CollectorId = collectorId, ScheduledFor = scheduledFor, RouteArea = routeArea });
        request.Status = RequestStatus.Scheduled;
        await database.SaveChangesAsync();
        TempData["Success"] = "Collection was scheduled.";
        return RedirectToPage(new { id });
    }
    public async Task<IActionResult> OnPostCompleteAsync(int id, string? completionNotes)
    {
        var request = await database.CollectionRequests.Include(item => item.Schedule).SingleOrDefaultAsync(item => item.Id == id);
        if (request is null) return NotFound();
        if (!User.IsInRole("Admin") && request.Schedule?.CollectorId.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid();
        if (request.Status != RequestStatus.Scheduled || request.Schedule is null)
        {
            TempData["Error"] = "Only a scheduled collection can be marked as completed.";
            return RedirectToPage(new { id });
        }
        request.Status = RequestStatus.Collected;
        request.Schedule.CompletedAtUtc = DateTime.UtcNow;
        request.Schedule.CompletionNotes = completionNotes?.Trim();
        await database.SaveChangesAsync();
        TempData["Success"] = "Collection marked as completed.";
        return RedirectToPage(new { id });
    }
    private Task<CollectionRequest?> LoadRequestAsync(int id) => database.CollectionRequests.Include(item => item.Resident).Include(item => item.Schedule).ThenInclude(schedule => schedule!.Collector).SingleOrDefaultAsync(item => item.Id == id);
    private async Task LoadCollectorsAsync() => CollectorOptions = await database.Users.Where(user => user.Role == UserRole.Collector && user.IsActive).OrderBy(user => user.FullName).Select(user => new SelectListItem(user.FullName, user.Id.ToString())).ToListAsync();
}
