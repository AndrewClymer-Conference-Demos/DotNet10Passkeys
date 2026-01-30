using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace DIgitalFrontDoor.Pages.Account
{
    public class LogoutModel(SignInManager<IdentityUser> signInManager) : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            await signInManager.SignOutAsync();
           
            return Redirect("/Account/SignedOut");
        }
    }
}
