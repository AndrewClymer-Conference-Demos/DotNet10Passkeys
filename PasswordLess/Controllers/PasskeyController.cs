using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PasswordLess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasskeyController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager) : ControllerBase
    {
        [HttpPost("CreatePassKeyOptions")]
        public async Task<IActionResult> CreatePassKeyOptions([FromBody]PasskeySetup passkeySetup)
        {
           return Problem("Not Implemented Yet",statusCode:501);
        }

        [HttpPost("CompletePassKeyRegistration")]
        public async Task<IActionResult> CompletePassKeyRegistration()
        {
            return Problem("Not Implemented Yet",statusCode:501);
        }

        [HttpPost("PasskeyRequestOptions")]
        public async Task<IActionResult> PasskeyRequestOptions([FromBody] PasskeyRequest request)
        {

            return Problem("Not Implemented Yet",statusCode:501);
        }
        
        [HttpPost("VerifyPasskey")]
        public async Task<IActionResult> Verify()
        {
            return Problem("Not Implemented Yet",statusCode:501);
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
