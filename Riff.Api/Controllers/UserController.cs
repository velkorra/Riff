using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Endpoints;
using Riff.Api.Services.Interfaces;

namespace Riff.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase, IUsersApi
{
    private readonly IUserService _userService;
    private readonly IResourceLinker _resourceLinker;

    public UsersController(IUserService userService, IResourceLinker resourceLinker)
    {
        _userService = userService;
        _resourceLinker = resourceLinker;
    }

    [HttpPost("register", Name = nameof(RegisterUser))]
    public async Task<ActionResult<UserResponse>> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var userDto = await _userService.RegisterAsync(request);
        var enrichedDto = _resourceLinker.AddLinksToUser(userDto);
        return CreatedAtAction(nameof(GetUserById), new { id = enrichedDto.Id }, enrichedDto);
    }

    [HttpGet("{id:guid}", Name = nameof(GetUserById))]
    public async Task<ActionResult<UserResponse>> GetUserById(Guid id)
    {
        var userDto = await _userService.GetByIdAsync(id);
        var enrichedDto = _resourceLinker.AddLinksToUser(userDto);
        return Ok(enrichedDto);
    }
}