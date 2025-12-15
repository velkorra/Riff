using System.Security.Claims;
using HotChocolate.Authorization;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Protos;
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
    [GraphQLDescription("Adds a track via gRPC.")]
    public async Task<TrackResponse> AddTrackToRoom(
        Guid roomId,
        Contracts.Dto.AddTrackRequest input,
        ClaimsPrincipal claimsPrincipal,
        [Service] Playlist.PlaylistClient playlistClient,
        [Service] ITrackService trackReader)
    {
        var userId = claimsPrincipal.GetUserId();

        var reply = await playlistClient.AddTrackAsync(new Contracts.Protos.AddTrackRequest
        {
            RoomId = roomId.ToString(),
            UserId = userId.ToString(),
            Title = input.Title,
            Artist = input.Artist,
            Url = input.Url,
            DurationSeconds = input.DurationInSeconds
        });

        if (!reply.Success) throw new GraphQLException(reply.ErrorMessage);

        return await trackReader.GetByIdAsync(Guid.Parse(reply.TrackId));
    }

    [Authorize]
    [GraphQLDescription("Vote for a track via gRPC.")]
    public async Task<bool> Vote(
        Guid trackId,
        int value,
        ClaimsPrincipal claimsPrincipal,
        [Service] Playlist.PlaylistClient playlistClient)
    {
        var userId = claimsPrincipal.GetUserId();

        var reply = await playlistClient.VoteAsync(new Contracts.Protos.VoteRequest
        {
            TrackId = trackId.ToString(),
            UserId = userId.ToString(),
            Value = value
        });

        if (!reply.Success) throw new GraphQLException(reply.ErrorMessage);

        return true;
    }

    [Authorize]
    [GraphQLDescription("Starts playback in the room.")]
    public async Task<bool> Play(
        Guid roomId,
        ClaimsPrincipal claimsPrincipal,
        [Service] Playlist.PlaylistClient playlistClient)
    {
        var userId = claimsPrincipal.GetUserId();
        var reply = await playlistClient.PlayAsync(new PlayerRequest
        {
            RoomId = roomId.ToString(),
            UserId = userId.ToString()
        });

        if (!reply.Success) throw new GraphQLException(reply.ErrorMessage);
        return true;
    }

    [Authorize]
    [GraphQLDescription("Pauses playback in the room.")]
    public async Task<bool> Pause(
        Guid roomId,
        ClaimsPrincipal claimsPrincipal,
        [Service] Playlist.PlaylistClient playlistClient)
    {
        var userId = claimsPrincipal.GetUserId();
        var reply = await playlistClient.PauseAsync(new PlayerRequest
        {
            RoomId = roomId.ToString(),
            UserId = userId.ToString()
        });

        if (!reply.Success) throw new GraphQLException(reply.ErrorMessage);
        return true;
    }

    [Authorize]
    [GraphQLDescription("Skips the current track.")]
    public async Task<bool> Skip(
        Guid roomId,
        ClaimsPrincipal claimsPrincipal,
        [Service] Playlist.PlaylistClient playlistClient)
    {
        var userId = claimsPrincipal.GetUserId();
        var reply = await playlistClient.SkipAsync(new PlayerRequest
        {
            RoomId = roomId.ToString(),
            UserId = userId.ToString()
        });

        if (!reply.Success) throw new GraphQLException(reply.ErrorMessage);
        return true;
    }

    [Authorize]
    [GraphQLDescription("Removes a track from the playlist.")]
    public async Task<bool> RemoveTrack(
        Guid trackId,
        ClaimsPrincipal claimsPrincipal,
        [Service] Playlist.PlaylistClient playlistClient)
    {
        var userId = claimsPrincipal.GetUserId();
        var reply = await playlistClient.RemoveTrackAsync(new RemoveTrackRequest
        {
            TrackId = trackId.ToString(),
            UserId = userId.ToString()
        });

        if (!reply.Success) throw new GraphQLException(reply.ErrorMessage);
        return true;
    }
}