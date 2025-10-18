using Microsoft.AspNetCore.Mvc;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Endpoints;
using Riff.ApiGateway.Services.Interfaces;

namespace Riff.ApiGateway.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomController: ControllerBase, IRoomsApi
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost]
    public async Task<ActionResult<RoomResponse>> CreateRoom([FromBody] CreateRoomRequest request)
    {
        // HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var currentUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var newRoom = await _roomService.Create(request, currentUserId);
        return CreatedAtAction(nameof(GetRoom), new { id = newRoom.Id }, newRoom);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomResponse>> GetRoom(Guid id)
    {
        return await _roomService.GetById(id);
    }
}
