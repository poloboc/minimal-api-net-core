namespace MinimalAPI.Controllers;

// [Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("[controller]")]
// Example how to use the API version as URL segment
// [Route("v{version:apiVersion}/[controller]")]
public class ControllerBaseClass : ControllerBase
{
    
}