using CleanRoute.Data;
using CleanRoute.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanRoute.Services;

public class RequestService(CleanRouteDbContext database)
{
    public Task<List<CollectionRequest>> GetForResidentAsync(int residentId) =>
        database.CollectionRequests.Where(request => request.ResidentId == residentId)
            .Include(request => request.Resident)
            .Include(request => request.Schedule).ThenInclude(schedule => schedule!.Collector)
            .OrderByDescending(request => request.CreatedAtUtc).ToListAsync();

    public Task<List<CollectionRequest>> GetOpenRequestsAsync() =>
        database.CollectionRequests.Include(request => request.Resident).Include(request => request.Schedule)
            .Where(request => request.Status == RequestStatus.Submitted || request.Status == RequestStatus.Scheduled)
            .OrderBy(request => request.PreferredDate).ToListAsync();

    public Task<List<CollectionRequest>> GetForCollectorAsync(int collectorId) =>
        database.CollectionRequests.Include(request => request.Resident)
            .Include(request => request.Schedule).ThenInclude(schedule => schedule!.Collector)
            .Where(request => request.Schedule != null && request.Schedule.CollectorId == collectorId)
            .OrderBy(request => request.Schedule!.ScheduledFor).ToListAsync();

    public async Task<CollectionRequest> CreateAsync(CollectionRequest request, int residentId)
    {
        request.ResidentId = residentId;
        request.Status = RequestStatus.Submitted;
        request.CreatedAtUtc = DateTime.UtcNow;
        database.CollectionRequests.Add(request);
        await database.SaveChangesAsync();
        return request;
    }
}
