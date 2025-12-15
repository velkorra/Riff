using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;

namespace Riff.Api.Contracts.Endpoints;

public interface IRoomsApi
{
    [EndpointSummary("Create a new room")]
    [EndpointDescription("Creates a room with the given name and optional password.")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    Task<ActionResult<RoomResponse>> CreateRoom([FromBody] CreateRoomRequest request);

    [EndpointSummary("Get a room by ID")]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    Task<ActionResult<RoomResponse>> GetRoomById(Guid id);

    [EndpointSummary("Get the playlist for a room")]
    [ProducesResponseType(typeof(IEnumerable<TrackResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    Task<ActionResult<IEnumerable<TrackResponse>>> GetRoomPlaylist(Guid roomId);

    [EndpointSummary("Add a track to a room's playlist")]
    [ProducesResponseType(typeof(TrackResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    Task<ActionResult<TrackResponse>> AddTrackToRoom(Guid roomId, [FromBody] AddTrackRequest request);
}