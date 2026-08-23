using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PasswordLess.Extensions;
using System.Text.Json;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace PasswordLess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasskeyController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager) : ControllerBase
    {
        [HttpPost("CreatePassKeyOptions")]
        public async Task<IActionResult> CreatePassKeyOptions([FromBody]PasskeySetup passkeySetup)
        {
            IdentityUser? user = await userManager.GetUserAsync(User);

            if (user?.UserName == null)
            {
                return BadRequest("User must be authenticaed to setup a passkey");
            }


            PasskeyUserEntity newPasskey = new PasskeyUserEntity()
            {
                DisplayName = passkeySetup.DeviceName,
                Id = user.Id,
                Name = user.UserName
            };

            string options = await signInManager.MakePasskeyCreationOptionsAsync(newPasskey);

            return Ok(options);
        }

        [HttpPost("CompletePassKeyRegistration")]
        public async Task<IActionResult> CompletePassKeyRegistration()
        {
            IdentityUser? user = await userManager.GetUserAsync(User);

            if (user?.UserName == null)
            {
                return BadRequest("User must be authenticaed to setup a passkey");
            }

            string credentials = await Request.ReadBodyAsStringAsync();
            
            PasskeyAttestationResult attestationResult = await signInManager.PerformPasskeyAttestationAsync(credentials);

            if (attestationResult.Succeeded == false)
            {
                return BadRequest($"Attestation failed , {attestationResult.Failure?.Message}");
            }

            attestationResult.Passkey.Name = attestationResult.UserEntity.DisplayName;
            IdentityResult createResult = await userManager.AddOrUpdatePasskeyAsync(user, attestationResult.Passkey);

            if (createResult.Succeeded == false)
            {
                return BadRequest($"Failed {createResult.Errors.First()?.Description}");
            }

            return Ok();
        }

        [HttpPost("PasskeyRequestOptions")]
        public async Task<IActionResult> PasskeyRequestOptions([FromBody] PasskeyRequest request)
        {
            IdentityUser? user = await userManager.FindByNameAsync(request.Username);

            string credentials = await signInManager.MakePasskeyRequestOptionsAsync(user);

            return Ok(credentials);
        }
        
        [HttpPost("VerifyPasskey")]
        public async Task<IActionResult> Verify()
        {
            string credentials = await Request.ReadBodyAsStringAsync();

            SignInResult result = await signInManager.PasskeySignInAsync(credentials);

            if (result.Succeeded == false)
            {
                byte[]? passkeyId = TryReadPasskeyId(credentials);

                if (passkeyId is not null)
                {
                    IdentityUser? existingUser = await userManager.FindByPasskeyIdAsync(passkeyId);

                    if (existingUser is null)
                    {
                        return NotFound("Passkey credential was not found.");
                    }
                }

                return BadRequest($"Failed to sign in");
            }

            return Ok();
        }

        private static byte[]? TryReadPasskeyId(string credentials)
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(credentials);

                if (document.RootElement.TryGetProperty("id", out JsonElement idElement) &&
                    string.IsNullOrWhiteSpace(idElement.GetString()) == false)
                {
                    return TryDecodePasskeyId(idElement.GetString()!);
                }

                if (document.RootElement.TryGetProperty("rawId", out JsonElement rawIdElement) &&
                    string.IsNullOrWhiteSpace(rawIdElement.GetString()) == false)
                {
                    return TryDecodePasskeyId(rawIdElement.GetString()!);
                }

                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static byte[]? TryDecodePasskeyId(string encodedPasskeyId)
        {
            try
            {
                return WebEncoders.Base64UrlDecode(encodedPasskeyId);
            }
            catch (FormatException)
            {
                try
                {
                    return Convert.FromBase64String(encodedPasskeyId);
                }
                catch (FormatException)
                {
                    return null;
                }
            }
        }
        
       
    }

    public class PasskeySetup
    {
        public string DeviceName { get; set; } = String.Empty;
    }

    public class PasskeyRequest
    {
        public string Username { get; set; } = String.Empty;
    }
}
