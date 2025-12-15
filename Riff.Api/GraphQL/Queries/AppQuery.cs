using System.Security.Claims;
using HotChocolate.Authorization;
using Riff.Api.Contracts.Dto;
using Riff.Api.Extensions;
using Riff.Api.Services.Interfaces;

namespace Riff.Api.GraphQL.Queries;

public class AppQuery
{
    [GraphQLDescription("Retrieves a user by their unique ID.")]
    public async Task<UserResponse> GetUserById(
        Guid id,
        [Service] IUserService userService)
    {
        return await userService.GetByIdAsync(id);
    }

    [GraphQLDescription("Retrieves the currently logged in user profile.")]
    [Authorize]
    public async Task<UserResponse> GetMe(
        ClaimsPrincipal claimsPrincipal,
        [Service] IUserService userService)
    {
        var id = claimsPrincipal.GetUserId();
        return await userService.GetByIdAsync(id);
    }

    [GraphQLDescription("Retrieves a room by its unique ID.")]
    public async Task<RoomResponse> GetRoomById(
        Guid id,
        [Service] IRoomService roomService)
    {
        return await roomService.GetByIdAsync(id);
    }

    [GraphQLDescription("Get top tracks globally.")]
    public async Task<IEnumerable<TrackResponse>> GetTopTracks(
        int limit,
        [Service] ITrackService trackService)
    {
        return await trackService.GetGlobalTopAsync(limit);
    }

    [GraphQLDescription("Get list of latest public rooms.")]
    public async Task<IEnumerable<RoomResponse>> GetPublicRooms(
        [Service] IRoomService roomService)
    {
        return await roomService.GetPublicRoomsAsync();
    }

    [GraphQLDescription("Retrieves a track by its unique ID.")]
    public async Task<TrackResponse> GetTrackById(
        Guid id,
        [Service] ITrackService trackService)
    {
        return await trackService.GetByIdAsync(id);
    }
}