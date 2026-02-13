using System.Security.Claims;
using DIgitalFrontDoor.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace DIgitalFrontDoor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasskeyController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager) : ControllerBase
    {

        [HttpPost("CreatePassKeyOptions")]
        public async Task<IActionResult> CreatePassKeyOptions([FromBody] PassKeyPayload payload)
        {
            IdentityUser? newUser = await userManager.FindByEmailAsync(payload.UserName);

            if (newUser == null)
            {
                newUser = new IdentityUser
                {
                    UserName = payload.UserName,
                    Email = payload.UserName,
                    EmailConfirmed = true
                };


                IdentityResult createResult = await userManager.CreateAsync(newUser);
            }

            await signInManager.SignInAsync(newUser,new AuthenticationProperties());
            
            var passKeyEntity = new PasskeyUserEntity()
            {
                Id = newUser.Id,
                DisplayName =  payload.DeviceName,
                Name = payload.UserName
            };

            
           string creationOptions = await signInManager.MakePasskeyCreationOptionsAsync(passKeyEntity);

           
            // Add your logic here
            return Ok(creationOptions);
        }

        [HttpPost("CompletePassKeyRegistration")]
        public async Task<IActionResult> CompletePassKeyRegistration()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return BadRequest("No activation session restart the registration process");
            }
         
            IdentityUser? user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("User account not found to bind passkey to");
            }
            
           string passkeyCredentialsAsJson = await Request.ReadBodyAsStringAsync();

           PasskeyAttestationResult attestationResult =
                await signInManager.PerformPasskeyAttestationAsync(passkeyCredentialsAsJson);

           if (!attestationResult.Succeeded)
           {
               return BadRequest(attestationResult.Failure?.Message ?? "Unknown Error");
           }
           
           var addResult = 
               await userManager.AddOrUpdatePasskeyAsync(user, attestationResult.Passkey);

           if (!addResult.Succeeded)
           {
               return BadRequest("Failed to store passkey");
           }
           
           
            return Ok();
        }

        [HttpPost("PasskeyRequestOptions")]
        public async Task<IActionResult> PasskeyRequestOptions(string? username)
        {

            IdentityUser? user = username != null ? await userManager.FindByNameAsync(username) : null;
            
            string requestOptions = await signInManager.MakePasskeyRequestOptionsAsync(user);

            return Ok(requestOptions);
        }
        
        [HttpPost("VerifyPasskey")]
        public async Task<IActionResult> Verify()
        {
            string credentials = await Request.ReadBodyAsStringAsync();

            SignInResult result = await signInManager.PasskeySignInAsync(credentials);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            return Ok();
        }
    }

    public class PassKeyPayload
    {
        public string UserName { get; set; }
        public string DeviceName { get; set; }
    }
}
