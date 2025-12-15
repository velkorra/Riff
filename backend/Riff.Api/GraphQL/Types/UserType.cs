using Riff.Api.Contracts.Dto;
using Riff.Api.Services.Interfaces;

namespace Riff.Api.GraphQL.Types;

public class UserType : ObjectType<UserResponse>
{
    protected override void Configure(IObjectTypeDescriptor<UserResponse> descriptor)
    {
        descriptor.Name("User");
        descriptor.Field(f => f.Id).Type<NonNullType<IdType>>();

        descriptor.Field("rooms")
            .ResolveWith<UserResolvers>(u => u.GetRoomsAsync(null!, null!));
    }

    private class UserResolvers
    {
        public async Task<IEnumerable<RoomResponse>> GetRoomsAsync(
            [Parent] UserResponse user,
            [Service] IRoomService roomService)
        {
            return await roomService.GetRoomsByOwnerIdAsync(user.Id);
        }
    }
}