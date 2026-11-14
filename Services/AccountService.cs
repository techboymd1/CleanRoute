using CleanRoute.Data;
using CleanRoute.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanRoute.Services;

public class AccountService(CleanRouteDbContext database, PasswordService passwords)
{
    public async Task<ApplicationUser?> ValidateCredentialsAsync(string email, string password)
    {
        var normalisedEmail = email.Trim().ToLowerInvariant();
        var user = await database.Users.SingleOrDefaultAsync(user => user.Email == normalisedEmail && user.IsActive);
        return user is not null && passwords.Verify(password, user.PasswordHash) ? user : null;
    }

    public async Task<(bool Succeeded, string? Error)> RegisterResidentAsync(ApplicationUser user, string password)
    {
        user.Email = user.Email.Trim().ToLowerInvariant();
        if (await database.Users.AnyAsync(existing => existing.Email == user.Email)) return (false, "An account already uses this email address.");
        user.PasswordHash = passwords.Hash(password);
        user.Role = UserRole.Resident;
        user.IsActive = true;
        user.CreatedAtUtc = DateTime.UtcNow;
        database.Users.Add(user);
        await database.SaveChangesAsync();
        return (true, null);
    }
}
