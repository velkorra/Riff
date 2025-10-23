using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Endpoints;
using Riff.Api.Services.Interfaces;

namespace Riff.Api.Controllers;

[ApiController]
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
        // TODO: Get OwnerId from authenticated user context 
        var hardcodedOwnerId = Guid.Parse("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6");
        
        var roomDto = await _roomService.CreateAsync(request, hardcodedOwnerId);
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
        // TODO: Get UserID from authenticated user context.
        var hardcodedUserId = Guid.Parse("b1c2d3e4-f5a6-b7c8-d9e0-f1a2b3c4d5e6");
        
        var trackDto = await _trackService.AddTrackAsync(roomId, request, hardcodedUserId);
        var enrichedDto = _resourceLinker.AddLinksToTrack(trackDto);

        // TODO: Create a dedicated "GetTrackById" endpoint to return a proper CreatedAtAction result.
        return StatusCode(StatusCodes.Status201Created, enrichedDto);
    }
}