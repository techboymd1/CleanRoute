using System.ComponentModel.DataAnnotations;

namespace CleanRoute.Models;

public class CollectionSchedule
{
    public int Id { get; set; }
    public int CollectionRequestId { get; set; }
    public CollectionRequest CollectionRequest { get; set; } = null!;
    public int CollectorId { get; set; }
    public ApplicationUser Collector { get; set; } = null!;
    public DateTime ScheduledFor { get; set; }
    [StringLength(250)] public string? RouteArea { get; set; }
    [StringLength(500)] public string? CompletionNotes { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}
