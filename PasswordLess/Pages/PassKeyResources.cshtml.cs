using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PasswordLess.Pages;

public class PassKeyResourcesModel(UserManager<IdentityUser> userManager) :PageModel
{
    public IList<UserPasskeyInfo> PassKeys { get; private set; } = [];

    public async Task OnGet()
    {
        IdentityUser? user = await userManager.GetUserAsync(User);
        if (user != null)
        {
            PassKeys = await userManager.GetPasskeysAsync(user);
        }
    }
}
