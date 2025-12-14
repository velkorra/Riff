using Riff.Api.Contracts.Dto;
using Riff.Api.Controllers;
using Riff.Api.Services.Interfaces;

namespace Riff.Api.Services;

public class ResourceLinker : IResourceLinker
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResourceLinker(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public UserResponse AddLinksToUser(UserResponse user)
    {
        var links = new List<LinkDto>
        {
            CreateLink(nameof(UsersController.GetUserById), new { id = user.Id }, "self", "GET")
        };
        return user with { Links = links };
    }

    public RoomResponse AddLinksToRoom(RoomResponse room)
    {
        var links = new List<LinkDto>
        {
            CreateLink(nameof(RoomsController.GetRoomById), new { id = room.Id }, "self", "GET"),
            CreateLink(nameof(UsersController.GetUserById), new { id = room.OwnerId }, "owner", "GET"),
            CreateLink(nameof(RoomsController.GetRoomPlaylist), new { roomId = room.Id }, "playlist", "GET")
        };
        return room with { Links = links };
    }

    public TrackResponse AddLinksToTrack(TrackResponse track)
    {
        var links = new List<LinkDto>
        {
            CreateLink(nameof(TracksController.GetTrackById), new { id = track.Id }, "self", "GET"),
            CreateLink(nameof(RoomsController.GetRoomById), new { id = track.RoomId }, "room", "GET"),
            CreateLink(nameof(UsersController.GetUserById), new { id = track.AddedById }, "addedBy", "GET")
        };
        return track with { Links = links };
    }

    private LinkDto CreateLink(string endpointName, object? values, string rel, string method)
    {
        var httpContext = _httpContextAccessor.HttpContext!;
        var href = _linkGenerator.GetUriByName(httpContext, endpointName, values)
                   ?? throw new InvalidOperationException($"Could not generate URL for endpoint '{endpointName}'.");
        return new LinkDto(href, rel, method);
    }
}