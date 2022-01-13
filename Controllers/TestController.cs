namespace MinimalAPI.Controllers;

/// <summary>
/// Test controller, documentation example
/// </summary>
public class TestController : ControllerBaseClass
{
    /// <summary>
    /// Xml Doc for the v1.0
    /// </summary>
    /// <response code="401">No Authorization Provided (Bearer)</response>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public ResponseModel DefaultGet10()
    {
        return new ResponseModel { Title = "Hello from v1" };
    }

    /// <summary>
    /// Xml Doc for the v1.1
    /// </summary>
    /// <response code="401">No Authorization Provided (Bearer)</response>
    /// <media type="application/json">sad asd asd asd </media>
    /// <returns></returns>
    [HttpGet]
    [ApiVersion("1.1")]
    [Route("/Test/{title}")]
    [Produces("application/json")]
    public ResponseModel DefaultGet11(string title)
    {
        return new ResponseModel { Title = title };
    }

    /// <summary>
    /// Xml Doc for the v2.0
    /// </summary>
    /// <response code="401">No Authorization Provided (Bearer)</response>
    /// <media type="text/plain">sad asd asd asd </media>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ApiVersion("2.0")]
    [Produces("text/plain")]
    [Consumes("application/json")]
    public string? DefaultPost20(FormModel test)
    {
        return test.Title;
    }
}