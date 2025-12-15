using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;

namespace Riff.Api.Controllers;

[ApiController]
[Route("/api")]
[ApiExplorerSettings(IgnoreApi = true)]
public class RootController(LinkGenerator linkGenerator) : ControllerBase
{
    [HttpGet(Name = "GetApiRoot")]
    public IActionResult GetApiRoot()
    {
        var links = new List<LinkDto>
        {
            new(linkGenerator.GetUriByName(HttpContext, "GetApiRoot", null)!, "self", "GET"),
            new(linkGenerator.GetUriByName(HttpContext, nameof(RoomsController.GetRoomById), new { id = Guid.Empty })!.Split(Guid.Empty.ToString())[0], "rooms", "GET"),
            new(linkGenerator.GetUriByName(HttpContext, nameof(UsersController.GetUserById), new { id = Guid.Empty })!.Split(Guid.Empty.ToString())[0], "users", "GET"),
            new(Url.Content("~/") + "swagger", "documentation", "GET")
        };

        return Ok(new { Links = links });
    }
}