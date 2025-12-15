using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;

namespace Riff.Api.Contracts.Endpoints;

public interface ITracksApi
{
    [EndpointSummary("Get top tracks globally")]
    [ProducesResponseType(typeof(IEnumerable<TrackResponse>), StatusCodes.Status200OK)]
    Task<ActionResult<IEnumerable<TrackResponse>>> GetTopTracks([FromQuery] int limit = 20);

    [EndpointSummary("Get a track by ID")]
    [ProducesResponseType(typeof(TrackResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    Task<ActionResult<TrackResponse>> GetTrackById(Guid id);

    [EndpointSummary("Vote for a track")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    Task<IActionResult> VoteForTrack(Guid id, [FromBody] VoteRequest request);

    [EndpointSummary("Delete a track")]
    [EndpointDescription("Removes a track from the playlist.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    Task<IActionResult> DeleteTrack(Guid id);
}