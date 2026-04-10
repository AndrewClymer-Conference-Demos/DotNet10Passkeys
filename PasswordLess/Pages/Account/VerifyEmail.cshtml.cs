using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace PasswordLess.Pages.Account;

public class VerifyEmail(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager , 
    IVerifyAccountOwnership verifyAccountOwnership)
    : PageModel
{
    
    [BindProperty]
    public string? Code { get; set; }
    
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPostAsync()
    {
        
        // Verify account ownership compare challenge to value in cookie
        string email = String.Empty;
        bool verified = !String.IsNullOrEmpty(Code) && 
                        verifyAccountOwnership.ValidateChallenge(Request, Code, out  email);
        if (!verified)
        {
            ModelState.AddModelError(string.Empty, "Email address not found in cookie.");
            return Page();
        }
        
        // Check if user exists
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser { UserName = email, Email = email };
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
        // Sign in the user
        await signInManager.SignInAsync(user, isPersistent: false);
        // Optionally, clear the cookie
        Response.Cookies.Delete("UserEmail");
        // Redirect to a page after sign-in (e.g., home or dashboard)
        return RedirectToPage("/Account/SetupPasskey");
    }
}