using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace Riff.Api.Contracts.Endpoints;

public interface ITracksApi
{
    [SwaggerOperation(Summary = "Get top tracks globally")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns a list of popular tracks.", typeof(IEnumerable<TrackResponse>))]
    Task<ActionResult<IEnumerable<TrackResponse>>> GetTopTracks([FromQuery] int limit = 20);

    [SwaggerOperation(Summary = "Get a track by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Track found and returned.", typeof(TrackResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Track not found.", typeof(ProblemDetails))]
    Task<ActionResult<TrackResponse>> GetTrackById(Guid id);

    [SwaggerOperation(Summary = "Vote for a track")]
    [SwaggerResponse(StatusCodes.Status200OK, "Vote accepted successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid vote value.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "User must be logged in to vote.")]
    Task<IActionResult> VoteForTrack(Guid id, [FromBody] VoteRequest request);
}