using CleanRoute.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanRoute.Data;

public class CleanRouteDbContext(DbContextOptions<CleanRouteDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<CollectionRequest> CollectionRequests => Set<CollectionRequest>();
    public DbSet<CollectionSchedule> CollectionSchedules => Set<CollectionSchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("users"); entity.Property(user => user.Id).HasColumnName("id");
            entity.Property(user => user.FullName).HasColumnName("full_name"); entity.Property(user => user.Email).HasColumnName("email");
            entity.Property(user => user.PasswordHash).HasColumnName("password_hash"); entity.Property(user => user.PhoneNumber).HasColumnName("phone_number");
            entity.Property(user => user.Address).HasColumnName("address"); entity.Property(user => user.Role).HasColumnName("role").HasConversion<string>();
            entity.Property(user => user.IsActive).HasColumnName("is_active"); entity.Property(user => user.CreatedAtUtc).HasColumnName("created_at_utc"); entity.HasIndex(user => user.Email).IsUnique();
        });
        modelBuilder.Entity<CollectionRequest>(entity =>
        {
            entity.ToTable("collection_requests"); entity.Property(request => request.Id).HasColumnName("id"); entity.Property(request => request.ResidentId).HasColumnName("resident_id");
            entity.Property(request => request.WasteType).HasColumnName("waste_type").HasConversion<string>(); entity.Property(request => request.CollectionAddress).HasColumnName("collection_address");
            entity.Property(request => request.Notes).HasColumnName("notes"); entity.Property(request => request.Status).HasColumnName("status").HasConversion<string>(); entity.Property(request => request.PreferredDate).HasColumnName("preferred_date"); entity.Property(request => request.CreatedAtUtc).HasColumnName("created_at_utc");
        });
        modelBuilder.Entity<CollectionRequest>().HasOne(request => request.Resident).WithMany(user => user.Requests)
            .HasForeignKey(request => request.ResidentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<CollectionSchedule>(entity =>
        {
            entity.ToTable("collection_schedules"); entity.Property(schedule => schedule.Id).HasColumnName("id"); entity.Property(schedule => schedule.CollectionRequestId).HasColumnName("collection_request_id");
            entity.Property(schedule => schedule.CollectorId).HasColumnName("collector_id"); entity.Property(schedule => schedule.ScheduledFor).HasColumnName("scheduled_for");
            entity.Property(schedule => schedule.RouteArea).HasColumnName("route_area"); entity.Property(schedule => schedule.CompletionNotes).HasColumnName("completion_notes"); entity.Property(schedule => schedule.CompletedAtUtc).HasColumnName("completed_at_utc"); entity.HasIndex(schedule => schedule.CollectionRequestId).IsUnique();
        });
        modelBuilder.Entity<CollectionSchedule>().HasOne(schedule => schedule.CollectionRequest).WithOne(request => request.Schedule)
            .HasForeignKey<CollectionSchedule>(schedule => schedule.CollectionRequestId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CollectionSchedule>().HasOne(schedule => schedule.Collector).WithMany(user => user.AssignedSchedules)
            .HasForeignKey(schedule => schedule.CollectorId).OnDelete(DeleteBehavior.Restrict);
    }
}
