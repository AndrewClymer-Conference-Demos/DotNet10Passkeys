using System.Diagnostics.CodeAnalysis;

namespace DIgitalFrontDoor.Pages.Account;

public interface IVerifyAccountOwnership
{
    void Challenge(HttpResponse response, string emailAddress);
    bool ValidateChallenge(HttpRequest request, string code,[NotNullWhen(true)] out string validatedEmailAddress);
}

public class CookieVerifyAccountOwnership : IVerifyAccountOwnership
{
    public void Challenge(HttpResponse response, string emailAddress)
    {
        // should validate that the email address does not contain a :, to ensure protection against code injection attack
        string code = "123";
        response.HttpContext.Session.SetString("UserEmail", emailAddress);
        response.HttpContext.Session.SetString("UserCode", code);
        // TODO: send code to email
    }

    public bool ValidateChallenge(HttpRequest request, string code, out string validatedEmailAddress)
    {
        validatedEmailAddress = string.Empty;
        string? sessionEmail = request.HttpContext.Session.GetString("UserEmail");
        string? sessionCode = request.HttpContext.Session.GetString("UserCode");
        if (sessionEmail == null || sessionCode == null) return false;
        if (code != sessionCode)
        {
            return false;
        }
        validatedEmailAddress = sessionEmail;
        return true;
    }
}