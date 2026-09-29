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
                return BadRequest("Must be authenticated to create a passkey");
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
                return BadRequest("Must be authenticated to create a passkey");
            }

            string credentials = await Request.ReadBodyAsStringAsync();

            PasskeyAttestationResult result = await signInManager.PerformPasskeyAttestationAsync(credentials);

            if (result.Succeeded == false)
            {
                return BadRequest("Not valid credentials");
            }

            // Must assign the name of the passkey
            result.Passkey.Name = result.UserEntity.DisplayName;
            
            var addPasskeyResult = await userManager.AddOrUpdatePasskeyAsync(user, result.Passkey);

            return addPasskeyResult.Succeeded ? Ok() : BadRequest("Failed to create key");
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
                return BadRequest("Failed to sign in");
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
