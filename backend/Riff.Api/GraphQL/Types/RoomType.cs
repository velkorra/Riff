using Riff.Api.Contracts.Dto;
using Riff.Api.Services.Interfaces;

namespace Riff.Api.GraphQL.Types;

public class RoomType : ObjectType<RoomResponse>
{
    protected override void Configure(IObjectTypeDescriptor<RoomResponse> descriptor)
    {
        descriptor.Name("Room");
        descriptor.Description("Represents a collaborative music room.");

        descriptor.Field(f => f.Id).Type<NonNullType<IdType>>();
        descriptor.Field(f => f.OwnerId).Type<NonNullType<IdType>>();

        descriptor.Field("owner")
            .Description("The user who owns this room.")
            .ResolveWith<RoomResolvers>(r => r.GetOwnerAsync(null!, null!));

        descriptor.Field("playlist")
            .Description("The list of tracks in this room.")
            .ResolveWith<RoomResolvers>(r => r.GetPlaylistAsync(null!, null!));
    }

    private class RoomResolvers
    {
        public async Task<UserResponse> GetOwnerAsync(
            [Parent] RoomResponse room,
            [Service] IUserService userService)
        {
            return await userService.GetByIdAsync(room.OwnerId);
        }

        public async Task<IEnumerable<TrackResponse>> GetPlaylistAsync(
            [Parent] RoomResponse room,
            [Service] ITrackService trackService)
        {
            return await trackService.GetPlaylistAsync(room.Id);
        }
    }
}