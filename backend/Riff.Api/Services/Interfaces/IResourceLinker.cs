using Riff.Api.Contracts.Dto;

namespace Riff.Api.Services.Interfaces;

public interface IResourceLinker
{
    UserResponse AddLinksToUser(UserResponse user);
    RoomResponse AddLinksToRoom(RoomResponse room);
    TrackResponse AddLinksToTrack(TrackResponse track);
}