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
           return Problem("Not Implemented Yet",statusCode:501);
        }

        [HttpPost("CompletePassKeyRegistration")]
        public async Task<IActionResult> CompletePassKeyRegistration()
        {
            return Problem("Not Implemented Yet",statusCode:501);
        }

        [HttpPost("PasskeyRequestOptions")]
        public async Task<IActionResult> PasskeyRequestOptions(string? username)
        {

            return Problem("Not Implemented Yet",statusCode:501);
        }
        
        [HttpPost("VerifyPasskey")]
        public async Task<IActionResult> Verify()
        {
            return Problem("Not Implemented Yet",statusCode:501);
        }
    }

    public class PassKeyPayload
    {
        public string UserName { get; set; } = String.Empty;
        public string DeviceName { get; set; } = String.Empty;
    }
}
