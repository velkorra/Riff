using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace Riff.Api.Contracts.Endpoints;

public interface IUsersApi
{
    [SwaggerOperation(Summary = "Register a new user")]
    [SwaggerResponse(StatusCodes.Status201Created, "User registered successfully.", typeof(UserResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid registration data provided.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "A user with this username already exists.",
        typeof(ProblemDetails))]
    Task<ActionResult<UserResponse>> RegisterUser([FromBody] RegisterUserRequest request);

    [SwaggerOperation(Summary = "Get a user by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "User found and returned.", typeof(UserResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "A user with the specified ID was not found.",
        typeof(ProblemDetails))]
    Task<ActionResult<UserResponse>> GetUserById(Guid id);
}