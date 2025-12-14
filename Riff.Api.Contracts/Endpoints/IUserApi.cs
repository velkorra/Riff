using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace Riff.Api.Contracts.Endpoints;

public interface IUsersApi
{
    [SwaggerOperation(Summary = "Get a user by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "User found and returned.", typeof(UserResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "A user with the specified ID was not found.",
        typeof(ProblemDetails))]
    Task<ActionResult<UserResponse>> GetUserById(Guid id);
}