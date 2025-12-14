using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Endpoints;
using Riff.Api.Extensions;
using Riff.Api.Services.Interfaces;

namespace Riff.Api.Controllers;

[ApiController]
[Route("api/tracks")]
public class TracksController : ControllerBase, ITracksApi
{
    private readonly ITrackService _trackService;
    private readonly IResourceLinker _resourceLinker;

    // private readonly Voting.VotingClient _votingClient;

    public TracksController(ITrackService trackService, IResourceLinker resourceLinker)
    {
        _trackService = trackService;
        _resourceLinker = resourceLinker;
    }

    [HttpGet(Name = nameof(GetTopTracks))]
    public async Task<ActionResult<IEnumerable<TrackResponse>>> GetTopTracks([FromQuery] int limit = 20)
    {
        var tracks = await _trackService.GetGlobalTopAsync(limit);
        var enriched = tracks.Select(t => _resourceLinker.AddLinksToTrack(t));
        return Ok(enriched);
    }

    [HttpGet("{id:guid}", Name = nameof(GetTrackById))]
    public async Task<ActionResult<TrackResponse>> GetTrackById(Guid id)
    {
        var track = await _trackService.GetByIdAsync(id);
        var enriched = _resourceLinker.AddLinksToTrack(track);
        return Ok(enriched);
    }

    [Authorize]
    [HttpPost("{id:guid}/vote", Name = "VoteForTrack")]
    public async Task<IActionResult> VoteForTrack(Guid id, [FromBody] VoteRequest request)
    {
        var userId = User.GetUserId();

        // grpc call
        
        return Ok(new 
        { 
            Message = $"Simulated vote {request.Value} for track {id} by user {userId}. Wait for gRPC implementation." 
        });
    }
}