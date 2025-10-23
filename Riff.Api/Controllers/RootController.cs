using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;

namespace Riff.Api.Controllers;

[ApiController]
[Route("/api")]
[ApiExplorerSettings(IgnoreApi = true)]
public class RootController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;

    public RootController(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    [HttpGet(Name = "GetApiRoot")]
    public IActionResult GetApiRoot()
    {
        var links = new List<LinkDto>
        {
            new(_linkGenerator.GetUriByName(HttpContext, "GetApiRoot", null)!, "self", "GET"),
            new(_linkGenerator.GetUriByName(HttpContext, nameof(RoomsController.GetRoomById), new { id = Guid.Empty })!.Split(Guid.Empty.ToString())[0], "rooms", "GET"),
            new(_linkGenerator.GetUriByName(HttpContext, nameof(UsersController.GetUserById), new { id = Guid.Empty })!.Split(Guid.Empty.ToString())[0], "users", "GET"),
            new(Url.Content("~/") + "swagger", "documentation", "GET")
        };

        return Ok(new { Links = links });
    }
}