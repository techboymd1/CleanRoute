using System.ComponentModel.DataAnnotations;

namespace CleanRoute.Models;

public class ApplicationUser
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string FullName { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(150)] public string Email { get; set; } = string.Empty;
    [Required] public string PasswordHash { get; set; } = string.Empty;
    [Required, StringLength(25)] public string PhoneNumber { get; set; } = string.Empty;
    [Required, StringLength(200)] public string Address { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Resident;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<CollectionRequest> Requests { get; set; } = new List<CollectionRequest>();
    public ICollection<CollectionSchedule> AssignedSchedules { get; set; } = new List<CollectionSchedule>();
}
