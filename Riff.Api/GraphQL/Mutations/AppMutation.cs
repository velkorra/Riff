using System.Security.Claims;
using HotChocolate.Authorization;
using Riff.Api.Contracts.Dto;
using Riff.Api.Extensions;
using Riff.Api.Services.Interfaces;

namespace Riff.Api.GraphQL.Mutations;

public class AppMutation
{
    [Authorize]
    [GraphQLDescription("Creates a new collaborative room.")]
    public async Task<RoomResponse> CreateRoom(
        CreateRoomRequest input,
        ClaimsPrincipal claimsPrincipal,
        [Service] IRoomService roomService)
    {
        var userId = claimsPrincipal.GetUserId();
        return await roomService.CreateAsync(input, userId);
    }

    [Authorize]
    [GraphQLDescription("Adds a track to a specific room's playlist.")]
    public async Task<TrackResponse> AddTrackToRoom(
        Guid roomId,
        AddTrackRequest input,
        ClaimsPrincipal claimsPrincipal,
        [Service] ITrackService trackService)
    {
        var userId = claimsPrincipal.GetUserId();
        return await trackService.AddTrackAsync(roomId, input, userId);
    }

    [Authorize]
    [GraphQLDescription("Vote for a track. Value must be 1 (like) or -1 (dislike).")]
    public async Task<bool> Vote(
        Guid trackId,
        int value,
        ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.GetUserId();

        // await votingClient.CastVoteAsync(...)
        
        return true; 
    }
}