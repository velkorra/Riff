using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Endpoints;
using Riff.Api.Contracts.Protos;
using Riff.Api.Extensions;
using Riff.Api.Services.Interfaces;

namespace Riff.Api.Controllers;

[ApiController]
[Route("api/tracks")]
public class TracksController(
    ITrackService trackReadService,
    Playlist.PlaylistClient playlistClient,
    IResourceLinker resourceLinker)
    : ControllerBase, ITracksApi
{
    [HttpGet(Name = nameof(GetTopTracks))]
    public async Task<ActionResult<IEnumerable<TrackResponse>>> GetTopTracks([FromQuery] int limit = 20)
    {
        var tracks = await trackReadService.GetGlobalTopAsync(limit);
        var enriched = tracks.Select(resourceLinker.AddLinksToTrack);
        return Ok(enriched);
    }

    [HttpGet("{id:guid}", Name = nameof(GetTrackById))]
    public async Task<ActionResult<TrackResponse>> GetTrackById(Guid id)
    {
        var track = await trackReadService.GetByIdAsync(id);
        var enriched = resourceLinker.AddLinksToTrack(track);
        return Ok(enriched);
    }

    [Authorize]
    [HttpPost("{id:guid}/vote", Name = "VoteForTrack")]
    public async Task<IActionResult> VoteForTrack(Guid id, [FromBody] Contracts.Dto.VoteRequest request)
    {
        var userId = User.GetUserId();
        var reply = await playlistClient.VoteAsync(new Contracts.Protos.VoteRequest
        {
            TrackId = id.ToString(),
            UserId = userId.ToString(),
            Value = request.Value
        });

        if (!reply.Success) return BadRequest(new { Error = reply.ErrorMessage });

        return Ok(new { NewScore = reply.NewScore });
    }

    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeleteTrack")]
    public async Task<IActionResult> DeleteTrack(Guid id)
    {
        var userId = User.GetUserId();
        var reply = await playlistClient.RemoveTrackAsync(new RemoveTrackRequest
        {
             TrackId = id.ToString(),
             UserId = userId.ToString()
        });

        if (!reply.Success) return BadRequest(new { Error = reply.ErrorMessage });

        return NoContent();
    }
}