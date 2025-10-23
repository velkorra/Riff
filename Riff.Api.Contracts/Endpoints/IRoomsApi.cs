using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace Riff.Api.Contracts.Endpoints;

public interface IRoomsApi
{
    [SwaggerOperation(Summary = "Create a new room")]
    [SwaggerResponse(StatusCodes.Status201Created, "The room was created successfully.", typeof(RoomResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data for room creation.", typeof(ProblemDetails))]
    Task<ActionResult<RoomResponse>> CreateRoom([FromBody] CreateRoomRequest request);

    [SwaggerOperation(Summary = "Get a room by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Room found and returned.", typeof(RoomResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "A room with the specified ID was not found.",
        typeof(ProblemDetails))]
    Task<ActionResult<RoomResponse>> GetRoomById(Guid id);

    [SwaggerOperation(Summary = "Get the playlist for a room")]
    [SwaggerResponse(StatusCodes.Status200OK, "The list of tracks in the room.", typeof(IEnumerable<TrackResponse>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "A room with the specified ID was not found.",
        typeof(ProblemDetails))]
    Task<ActionResult<IEnumerable<TrackResponse>>> GetRoomPlaylist(Guid roomId);

    [SwaggerOperation(Summary = "Add a track to a room's playlist")]
    [SwaggerResponse(StatusCodes.Status201Created, "The track was successfully added to the room.",
        typeof(TrackResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "A room with the specified ID was not found.",
        typeof(ProblemDetails))]
    Task<ActionResult<TrackResponse>> AddTrackToRoom(Guid roomId, [FromBody] AddTrackRequest request);
}