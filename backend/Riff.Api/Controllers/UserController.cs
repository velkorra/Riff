using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Endpoints;
using Riff.Api.Extensions;
using Riff.Api.Services.Interfaces;
using IUserService = Riff.Api.Services.Interfaces.IUserService;

namespace Riff.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService, IResourceLinker resourceLinker) : ControllerBase, IUsersApi
{
    [HttpGet("{id:guid}", Name = nameof(GetUserById))]
    public async Task<ActionResult<UserResponse>> GetUserById(Guid id)
    {
        var userDto = await userService.GetByIdAsync(id);
        var enrichedDto = resourceLinker.AddLinksToUser(userDto);
        return Ok(enrichedDto);
    }

    [HttpGet("me", Name = "GetMe")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var myId = User.GetUserId();

        var userDto = await userService.GetByIdAsync(myId);

        var enrichedDto = resourceLinker.AddLinksToUser(userDto);

        return Ok(enrichedDto);
    }
}