using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;

namespace Riff.Api.Contracts.Endpoints;

public interface IUsersApi
{
    [EndpointSummary("Get a user by ID")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    Task<ActionResult<UserResponse>> GetUserById(Guid id);
}