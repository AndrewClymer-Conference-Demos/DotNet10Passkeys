using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PasswordLess.Extensions;

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
                return BadRequest("User not signed in");
            }

            PasskeyUserEntity newPassKey = new()
            {
                DisplayName = passkeySetup.DeviceName,
                Id = user.Id,
                Name = user.UserName
            };

            string credentials = await signInManager.MakePasskeyCreationOptionsAsync(newPassKey);

            return Ok(credentials);
        }

        [HttpPost("CompletePassKeyRegistration")]
        public async Task<IActionResult> CompletePassKeyRegistration()
        {
            IdentityUser? user = await userManager.GetUserAsync(User);
            if (user?.UserName == null)
            {
                return BadRequest("User not signed in");
            }

            string credentials = await Request.ReadBodyAsStringAsync();

            PasskeyAttestationResult attestationResult = await signInManager.PerformPasskeyAttestationAsync(credentials);
            if (attestationResult.Succeeded == false)
            {
                return BadRequest("Attestation result failed");
            }

            attestationResult.Passkey.Name = attestationResult.UserEntity.DisplayName;
            
            IdentityResult result = await userManager.AddOrUpdatePasskeyAsync(user, attestationResult.Passkey);

            if (result.Succeeded == false)
            {
                return BadRequest("Failed to create key");
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

            var signInResult = await signInManager.PasskeySignInAsync(credentials);
            if (signInResult.Succeeded == false)
            {
                return BadRequest($"Failed to sign");
            }

            return Ok();
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
