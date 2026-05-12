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
                return BadRequest("Not authenticated, need to be to setup a Passkey");
            }

            PasskeyUserEntity newPasskey = new PasskeyUserEntity()
            {
                DisplayName = passkeySetup.DeviceName,
                Id = user.Id,
                Name = user.UserName,
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
                return BadRequest("Not authenticated, need to be to setup a Passkey");
            }

            string credentials = await Request.ReadBodyAsStringAsync();
            PasskeyAttestationResult attestationResult =  await signInManager.PerformPasskeyAttestationAsync(credentials);
            if (attestationResult.Succeeded == false)
            {
                return BadRequest("Registration failed");
            }

            // this sucks...
            attestationResult.Passkey.Name = attestationResult.UserEntity.DisplayName;

            IdentityResult result = await userManager.AddOrUpdatePasskeyAsync(user, attestationResult.Passkey);

            return result.Succeeded ? Ok() : BadRequest("Failed to add key");
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
                return BadRequest("Failed to authenticarte");
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
