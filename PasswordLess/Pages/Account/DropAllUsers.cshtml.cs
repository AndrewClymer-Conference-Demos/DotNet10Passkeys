using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordLess.Storage;

namespace PasswordLess.Pages.Account;

public class DropAllUsersModel(ApplicationDbContext ctx, ILogger<DropAllUsersModel> logger) : PageModel
{
    public void OnGet()
    {
        // This allows the page to be accessed via GET for rendering after POST
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogInformation("Dropping all users from the database");
        await ctx.Reset();
        logger.LogInformation("All users have been dropped successfully");
        
        return Page();
    }
}
