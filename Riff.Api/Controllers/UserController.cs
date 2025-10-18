using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Endpoints;
using Riff.Api.Services.Interfaces;

namespace Riff.ApiGateway.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase, IUsersApi
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var newUser = await _userService.Create(request);
        return StatusCode(201, newUser);
    }
}