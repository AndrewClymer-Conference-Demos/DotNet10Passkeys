using DIgitalFrontDoor.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DIgitalFrontDoor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasskeyController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager) : ControllerBase
    {
        [HttpPost("CreatePassKeyOptions")]
        public async Task<IActionResult> CreatePassKeyOptions([FromBody]PasskeySetup passkeySetup)
        {
            IdentityUser? user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("Must be authenticated to setup a Passkey");
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
            if (user == null)
            {
                return BadRequest("Must be authenticated to setup a Passkey");
            }
            
            string credentials = await Request.ReadBodyAsStringAsync();

            PasskeyAttestationResult attestationResult = await signInManager.PerformPasskeyAttestationAsync(credentials);
            if (attestationResult.Succeeded == false)
            {
                return BadRequest("Bad Actor go away");
            }

            attestationResult.Passkey.Name = attestationResult.UserEntity.DisplayName;
            
            var addPasskeyResult = await userManager.AddOrUpdatePasskeyAsync(user, attestationResult.Passkey);

            return addPasskeyResult.Succeeded ? Ok() : BadRequest("Failed to save passkey");
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

            var signInResult = await signInManager.PasskeySignInAsync(credentials);

            return signInResult.Succeeded ? Ok() : BadRequest("Failed to authenticate");
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
