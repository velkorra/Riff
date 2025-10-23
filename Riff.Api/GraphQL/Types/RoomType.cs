using Application.Services.Interfaces;
using GraphQL.Types;
using Riff.Api.Contracts.Dto;

namespace Riff.Api.GraphQL.Types;

public sealed class RoomType : ObjectGraphType<RoomResponse>
{
    public RoomType()
    {
        Name = "Room";
        Description = "Represents a collaborative music room.";

        Field(r => r.Id, type: typeof(IdGraphType))
            .Description("The unique ID of the room.");

        Field(r => r.Name)
            .Description("The name of the room.");

        Field(r => r.OwnerId, type: typeof(IdGraphType))
            .Description("The ID of the user who owns the room.");

        Field(r => r.CreatedAt, type: typeof(DateTimeOffsetGraphType))
            .Description("The date and time the room was created.");

        Field<UserType>("owner")
            .Description("The user who owns this room.")
            .ResolveAsync(async context =>
            {
                var userService = context.RequestServices!.GetRequiredService<IUserService>();
                return await userService.GetByIdAsync(context.Source.OwnerId);
            });
            
        Field<ListGraphType<TrackType>>("playlist")
            .Description("The list of tracks in this room's playlist.")
            .ResolveAsync(async context =>
            {
                var trackService = context.RequestServices!.GetRequiredService<ITrackService>();
                return await trackService.GetPlaylistAsync(context.Source.Id);
            });
    }
}