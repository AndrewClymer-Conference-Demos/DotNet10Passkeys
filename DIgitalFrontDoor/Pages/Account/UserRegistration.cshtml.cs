using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DIgitalFrontDoor.Pages.Account;

public class UserRegistration(IVerifyAccountOwnership verifyAccountOwnership): PageModel
{
    [BindProperty]
    public string? EmailAddress { get; set; }

    public void OnGet()
    {
        
    }

    public IActionResult OnPost()
    {
        // Store the email address in a cookie for passing state to the next page
        if (!string.IsNullOrEmpty(EmailAddress))
        {
           verifyAccountOwnership.Challenge(Response,EmailAddress);
        }
        // Redirect to VerifyEmail page
        return RedirectToPage("/Account/VerifyEmail");
    }
}