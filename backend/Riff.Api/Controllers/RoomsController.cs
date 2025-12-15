using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Endpoints;
using Riff.Api.Contracts.Protos;
using Riff.Api.Extensions;
using Riff.Api.Services.Interfaces;
using IRoomService = Riff.Api.Services.Interfaces.IRoomService;
using ITrackService = Riff.Api.Services.Interfaces.ITrackService;

namespace Riff.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/rooms")]
public class RoomsController(
    IRoomService roomService,
    ITrackService trackService,
    IResourceLinker resourceLinker,
    Playlist.PlaylistClient playlistClient)
    : ControllerBase, IRoomsApi
{
    [HttpPost(Name = nameof(CreateRoom))]
    public async Task<ActionResult<RoomResponse>> CreateRoom([FromBody] CreateRoomRequest request)
    {
        var ownerId = User.GetUserId();

        var roomDto = await roomService.CreateAsync(request, ownerId);
        var enrichedDto = resourceLinker.AddLinksToRoom(roomDto);

        return CreatedAtAction(nameof(GetRoomById), new { id = enrichedDto.Id }, enrichedDto);
    }

    [HttpGet("{id:guid}", Name = nameof(GetRoomById))]
    public async Task<ActionResult<RoomResponse>> GetRoomById(Guid id)
    {
        var roomDto = await roomService.GetByIdAsync(id);
        var enrichedDto = resourceLinker.AddLinksToRoom(roomDto);
        return Ok(enrichedDto);
    }

    [HttpGet("{roomId:guid}/playlist", Name = nameof(GetRoomPlaylist))]
    public async Task<ActionResult<IEnumerable<TrackResponse>>> GetRoomPlaylist(Guid roomId)
    {
        var playlistDtos = await trackService.GetPlaylistAsync(roomId);
        var enrichedDtos = playlistDtos.Select(t => resourceLinker.AddLinksToTrack(t));
        return Ok(enrichedDtos);
    }

    [HttpGet(Name = "GetPublicRooms")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RoomResponse>>> GetPublicRooms(int limit = 20)
    {
        var rooms = await roomService.GetPublicRoomsAsync();
        return Ok(rooms.Select(resourceLinker.AddLinksToRoom));
    }

    [HttpPost("{roomId:guid}/playlist", Name = nameof(AddTrackToRoom))]
    public async Task<ActionResult<TrackResponse>> AddTrackToRoom(Guid roomId,
        [FromBody] Contracts.Dto.AddTrackRequest request)
    {
        var userId = User.GetUserId();

        var grpcReq = new Contracts.Protos.AddTrackRequest
        {
            RoomId = roomId.ToString(),
            UserId = userId.ToString(),
            Title = request.Title,
            Artist = request.Artist,
            Url = request.Url,
            DurationSeconds = request.DurationInSeconds
        };

        var reply = await playlistClient.AddTrackAsync(grpcReq);

        if (!reply.Success)
        {
            return BadRequest(new { Error = reply.ErrorMessage });
        }

        var newTrack = await trackService.GetByIdAsync(Guid.Parse(reply.TrackId));
        return StatusCode(201, resourceLinker.AddLinksToTrack(newTrack));
    }

    [Authorize]
    [HttpPost("{roomId:guid}/play")]
    public async Task<IActionResult> Play(Guid roomId)
    {
        var reply = await playlistClient.PlayAsync(new PlayerRequest
        {
            RoomId = roomId.ToString(),
            UserId = User.GetUserId().ToString()
        });

        if (!reply.Success) return BadRequest(new { Error = reply.ErrorMessage });
        return Ok(new { Status = reply.Status, TrackId = reply.CurrentTrackId });
    }

    [Authorize]
    [HttpPost("{roomId:guid}/pause")]
    public async Task<IActionResult> Pause(Guid roomId)
    {
        var reply = await playlistClient.PauseAsync(new PlayerRequest
        {
            RoomId = roomId.ToString(),
            UserId = User.GetUserId().ToString()
        });

        if (!reply.Success) return BadRequest(new { Error = reply.ErrorMessage });
        return Ok(new { Status = reply.Status, TrackId = reply.CurrentTrackId });
    }

    [Authorize]
    [HttpPost("{roomId:guid}/skip")]
    public async Task<IActionResult> Skip(Guid roomId)
    {
        var reply = await playlistClient.SkipAsync(new PlayerRequest
        {
            RoomId = roomId.ToString(),
            UserId = User.GetUserId().ToString()
        });

        if (!reply.Success) return BadRequest(new { Error = reply.ErrorMessage });
        return Ok(new { Status = reply.Status, TrackId = reply.CurrentTrackId });
    }
}