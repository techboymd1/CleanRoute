using CleanRoute.Data;
using CleanRoute.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
namespace CleanRoute.Pages.Admin.Users;

public class IndexModel(CleanRouteDbContext database) : PageModel
{
    public List<ApplicationUser> Users { get; private set; } = [];
    public async Task OnGetAsync() => Users = await database.Users.OrderBy(user => user.Role).ThenBy(user => user.FullName).ToListAsync();
}
