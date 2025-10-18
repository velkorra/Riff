using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace Riff.Api.Contracts.Endpoints;

public interface IUsersApi
{
    [SwaggerOperation(Summary = "Register a new user")]
    [SwaggerResponse(StatusCodes.Status201Created, "User registered successfully", typeof(UserResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid registration data", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Username is already taken", typeof(ProblemDetails))]
    Task<ActionResult<UserResponse>> RegisterUser([FromBody] RegisterUserRequest request);
}
