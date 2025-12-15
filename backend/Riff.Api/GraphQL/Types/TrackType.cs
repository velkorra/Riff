using Riff.Api.Contracts.Dto;
using Riff.Api.Services.Interfaces;

namespace Riff.Api.GraphQL.Types;

public class TrackType : ObjectType<TrackResponse>
{
    protected override void Configure(IObjectTypeDescriptor<TrackResponse> descriptor)
    {
        descriptor.Name("Track");
        descriptor.Description("Represents a track in a room's playlist.");

        descriptor.Field(f => f.Id).Type<NonNullType<IdType>>();
        descriptor.Field(f => f.RoomId).Type<NonNullType<IdType>>();
        descriptor.Field(f => f.AddedById).Type<NonNullType<IdType>>();

        descriptor.Field(f => f.Score)
            .Description("Current vote score of the track.");

        descriptor.Field("addedBy")
            .ResolveWith<TrackResolvers>(t => t.GetAddedByAsync(null!, null!));
    }

    private class TrackResolvers
    {
        public async Task<UserResponse> GetAddedByAsync(
            [Parent] TrackResponse track,
            [Service] IUserService userService)
        {
            return await userService.GetByIdAsync(track.AddedById);
        }
    }
}