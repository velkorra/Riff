using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Endpoints;
using Riff.Api.Extensions;
using Riff.Api.Services.Interfaces;
using IRoomService = Riff.Api.Services.Interfaces.IRoomService;
using ITrackService = Riff.Api.Services.Interfaces.ITrackService;

namespace Riff.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/rooms")]
public class RoomsController : ControllerBase, IRoomsApi
{
    private readonly IRoomService _roomService;
    private readonly ITrackService _trackService;
    private readonly IResourceLinker _resourceLinker;

    public RoomsController(IRoomService roomService, ITrackService trackService, IResourceLinker resourceLinker)
    {
        _roomService = roomService;
        _trackService = trackService;
        _resourceLinker = resourceLinker;
    }

    [HttpPost(Name = nameof(CreateRoom))]
    public async Task<ActionResult<RoomResponse>> CreateRoom([FromBody] CreateRoomRequest request)
    {
        var ownerId = User.GetUserId();

        var roomDto = await _roomService.CreateAsync(request, ownerId);
        var enrichedDto = _resourceLinker.AddLinksToRoom(roomDto);

        return CreatedAtAction(nameof(GetRoomById), new { id = enrichedDto.Id }, enrichedDto);
    }

    [HttpGet("{id:guid}", Name = nameof(GetRoomById))]
    public async Task<ActionResult<RoomResponse>> GetRoomById(Guid id)
    {
        var roomDto = await _roomService.GetByIdAsync(id);
        var enrichedDto = _resourceLinker.AddLinksToRoom(roomDto);
        return Ok(enrichedDto);
    }

    [HttpGet("{roomId:guid}/playlist", Name = nameof(GetRoomPlaylist))]
    public async Task<ActionResult<IEnumerable<TrackResponse>>> GetRoomPlaylist(Guid roomId)
    {
        var playlistDtos = await _trackService.GetPlaylistAsync(roomId);
        var enrichedDtos = playlistDtos.Select(t => _resourceLinker.AddLinksToTrack(t));
        return Ok(enrichedDtos);
    }

    [HttpPost("{roomId:guid}/playlist", Name = nameof(AddTrackToRoom))]
    public async Task<ActionResult<TrackResponse>> AddTrackToRoom(Guid roomId, [FromBody] AddTrackRequest request)
    {
        var userId = User.GetUserId();

        var trackDto = await _trackService.AddTrackAsync(roomId, request, userId);
        var enrichedDto = _resourceLinker.AddLinksToTrack(trackDto);

        return StatusCode(StatusCodes.Status201Created, enrichedDto);
    }
}