using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PasswordLess.Extensions;
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
                return BadRequest("Must be authenticated and have a username to setup a passkey");
            }
            
            PasskeyUserEntity newPasskey = new PasskeyUserEntity()
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
                return BadRequest("Must be authenticated and have a username to setup a passkey");
            }
            
            string credentials = await Request.ReadBodyAsStringAsync();

            PasskeyAttestationResult attestationResult = await signInManager.PerformPasskeyAttestationAsync(credentials);

            if (attestationResult.Succeeded is not true)
            {
                return BadRequest("Attestation verification failed");
            }

            attestationResult.Passkey.Name = attestationResult.UserEntity.DisplayName;

            IdentityResult result = await userManager.AddOrUpdatePasskeyAsync(user, attestationResult.Passkey);


            return result.Succeeded ? Ok() : BadRequest("Failed to create passkey");
        }

        [HttpPost("PasskeyRequestOptions")]
        public async Task<IActionResult> PasskeyRequestOptions([FromBody] PasskeyRequest request)
        {
            IdentityUser? user = await userManager.FindByNameAsync(request.Username);
            var challenge = await signInManager.MakePasskeyRequestOptionsAsync(user);
            return Ok(challenge);
        }
        
        [HttpPost("VerifyPasskey")]
        public async Task<IActionResult> Verify()
        {
            string credentials = await Request.ReadBodyAsStringAsync();

            SignInResult result = await signInManager.PasskeySignInAsync(credentials);
            
            return result.Succeeded ? Ok() : BadRequest("Failed to authenticated passkey");
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
