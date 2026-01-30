using Microsoft.AspNetCore.Mvc;

namespace DIgitalFrontDoor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasskeyController : ControllerBase
    {
        // ...existing code...

        [HttpPost("CreatePassKeyOptins")]
        public IActionResult CreatePassKeyOptins()
        {
            // Your logic here
            return Ok(new { Message = "PassKey options created successfully." });
        }

        // ...existing code...
    }
}
