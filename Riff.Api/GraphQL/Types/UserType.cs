using Application.Services.Interfaces;
using GraphQL.Types;
using Riff.Api.Contracts.Dto;

namespace Riff.Api.GraphQL.Types;

public sealed class UserType : ObjectGraphType<UserResponse>
{
    public UserType()
    {
        Name = "User";
        Description = "Represents a user in the system.";

        Field(u => u.Id, type: typeof(IdGraphType))
            .Description("The unique ID of the user.");

        Field(u => u.Username)
            .Description("The user's display name.");

        Field<ListGraphType<RoomType>>("rooms")
            .Description("The list of rooms owned by this user.")
            .ResolveAsync(async context =>
            {
                var roomService = context.RequestServices!.GetRequiredService<IRoomService>();
                return await roomService.GetRoomsByOwnerIdAsync(context.Source.Id);
            });
    }
}