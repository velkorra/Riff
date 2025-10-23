using Application.Services.Interfaces;
using GraphQL;
using GraphQL.Types;
using Riff.Api.Contracts.Dto;
using Riff.Api.GraphQL.Types;
using Riff.Api.GraphQL.Types.Input;

namespace Riff.Api.GraphQL;

public sealed class AppMutation : ObjectGraphType
{
    public AppMutation()
    {
        Name = "Mutation";
        Description = "Defines all available mutations.";

        Field<UserType>("registerUser")
            .Description("Registers a new user in the system.")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<RegisterUserInputType>> { Name = "input" }
            ))
            .ResolveAsync(async context =>
            {
                var input = context.GetArgument<RegisterUserRequest>("input");
                var userService = context.RequestServices!.GetRequiredService<IUserService>();
                return await userService.RegisterAsync(input);
            });
            
        Field<RoomType>("createRoom")
            .Description("Creates a new collaborative room.")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<CreateRoomInputType>> { Name = "input" }
            ))
            .ResolveAsync(async context =>
            {
                var input = context.GetArgument<CreateRoomRequest>("input");
                var roomService = context.RequestServices!.GetRequiredService<IRoomService>();
                // TODO: Get OwnerId from authenticated user context.
                var hardcodedOwnerId = Guid.Parse("a1b2c3d4-e5f6-a7b8-c9d0-e1f2a3b4c5d6");
                return await roomService.CreateAsync(input, hardcodedOwnerId);
            });
            
        Field<TrackType>("addTrackToRoom")
            .Description("Adds a track to a specific room's playlist.")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "roomId" },
                new QueryArgument<NonNullGraphType<AddTrackInputType>> { Name = "input" }
            ))
            .ResolveAsync(async context =>
            {
                var roomId = context.GetArgument<Guid>("roomId");
                var input = context.GetArgument<AddTrackRequest>("input");
                var trackService = context.RequestServices!.GetRequiredService<ITrackService>();
                // TODO: Get UserID from authenticated user context.
                var hardcodedUserId = Guid.Parse("b1c2d3e4-f5a6-b7c8-d9e0-f1a2b3c4d5e6");
                return await trackService.AddTrackAsync(roomId, input, hardcodedUserId);
            });
    }
}