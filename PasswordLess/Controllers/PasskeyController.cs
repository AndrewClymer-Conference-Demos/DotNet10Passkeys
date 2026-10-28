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
                return Unauthorized("Need to sign in to create Passkeys");
            }

            PasskeyUserEntity newPasskey = new()
            {
                Id = user.Id,
                Name = user.UserName,
                DisplayName = passkeySetup.DeviceName
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
                return Unauthorized("Need to sign in to create Passkeys");
            }

            string credentials = await Request.ReadBodyAsStringAsync();

            PasskeyAttestationResult attestationResult = await
                signInManager.PerformPasskeyAttestationAsync(credentials);

            if (attestationResult.Succeeded == false)
            {
                return BadRequest($"Failed to verify passkey registration {attestationResult.Failure.Message}");
            }

            attestationResult.Passkey.Name = attestationResult.UserEntity.DisplayName;
            
            var result = await userManager.AddOrUpdatePasskeyAsync(user, attestationResult.Passkey);

            return result.Succeeded ? Ok() : BadRequest("Failed to add passkey");
        }

        [HttpPost("PasskeyRequestOptions")]
        public async Task<IActionResult> PasskeyRequestOptions([FromBody] PasskeyRequest request)
        {
            IdentityUser? user = await userManager.FindByNameAsync(request.Username);

            string options = await signInManager.MakePasskeyRequestOptionsAsync(user);

            return Ok(options);
        }
        
        [HttpPost("VerifyPasskey")]
        public async Task<IActionResult> Verify()
        {
            string credentials = await Request.ReadBodyAsStringAsync();

            var result = await signInManager.PasskeySignInAsync(credentials);

            if (result.Succeeded == false)
            {
                return BadRequest("failed to sign in");
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
