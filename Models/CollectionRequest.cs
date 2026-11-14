using System.ComponentModel.DataAnnotations;

namespace CleanRoute.Models;

public class CollectionRequest
{
    public int Id { get; set; }
    public int ResidentId { get; set; }
    public ApplicationUser Resident { get; set; } = null!;
    [Required] public WasteType WasteType { get; set; }
    [Required, StringLength(500)] public string CollectionAddress { get; set; } = string.Empty;
    [StringLength(500)] public string? Notes { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Submitted;
    public DateTime PreferredDate { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public CollectionSchedule? Schedule { get; set; }
}
