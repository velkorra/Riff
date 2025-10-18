using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace Riff.Api.Contracts.Endpoints;

public interface IRoomsApi
{
    [SwaggerOperation(
        Summary = "Create a new room",
        Description = "Creates a new collaboration room for users to join."
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "The room was created successfully.", typeof(RoomResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The request is invalid.", typeof(StatusResponse))]
    Task<ActionResult<RoomResponse>> CreateRoom([FromBody] CreateRoomRequest request);

    [SwaggerOperation(
        Summary = "Get room by ID",
        Description = "Retrieves detailed information about a specific room."
    )]
    [SwaggerResponse(StatusCodes.Status404NotFound, "A room with the specified ID was not found.", typeof(StatusResponse))]
    Task<ActionResult<RoomResponse>> GetRoom(Guid id);
}