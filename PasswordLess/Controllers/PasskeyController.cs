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
                return BadRequest("Need to be authenticated");
            }

            PasskeyUserEntity newPassKey = new PasskeyUserEntity()
            {
                DisplayName = passkeySetup.DeviceName,
                Id = user.Id,
                Name = user.UserName
            };

            string options = await signInManager.MakePasskeyCreationOptionsAsync(newPassKey);

            return Ok(options);
        }

        [HttpPost("CompletePassKeyRegistration")]
        public async Task<IActionResult> CompletePassKeyRegistration()
        {
            IdentityUser? user = await userManager.GetUserAsync(User);
            if (user?.UserName == null)
            {
                return BadRequest("Need to be authenticated");
            }

            string credentials = await Request.ReadBodyAsStringAsync();
            PasskeyAttestationResult result = await signInManager.PerformPasskeyAttestationAsync(credentials);
            if (result.Succeeded == false)
            {
                return BadRequest("Failed to create credential");
            }

            // this sucks!!!
            result.Passkey.Name = result.UserEntity.DisplayName;
            
            IdentityResult addResult = await userManager.AddOrUpdatePasskeyAsync(user, result.Passkey);



            return addResult.Succeeded ? Ok() : BadRequest("Failed to add user");
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

            SignInResult result = await signInManager.PasskeySignInAsync(credentials);
            if (result.Succeeded == false)
            {
                return BadRequest("Not signed in");
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
