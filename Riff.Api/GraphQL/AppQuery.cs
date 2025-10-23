using Application.Services.Interfaces;
using GraphQL;
using GraphQL.Types;
using Riff.Api.GraphQL.Types;

namespace Riff.Api.GraphQL;

public sealed class AppQuery : ObjectGraphType
{
    public AppQuery()
    {
        Name = "Query";
        Description = "Defines all available queries.";

        Field<UserType>("userById")
            .Description("Retrieves a user by their unique ID.")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id", Description = "The ID of the user." }
            ))
            .ResolveAsync(async context =>
            {
                var id = context.GetArgument<Guid>("id");
                var userService = context.RequestServices!.GetRequiredService<IUserService>();
                return await userService.GetByIdAsync(id);
            });
            
        Field<RoomType>("roomById")
            .Description("Retrieves a room by its unique ID.")
            .Arguments(new QueryArguments(
                new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id", Description = "The ID of the room." }
            ))
            .ResolveAsync(async context =>
            {
                var id = context.GetArgument<Guid>("id");
                var roomService = context.RequestServices!.GetRequiredService<IRoomService>();
                return await roomService.GetByIdAsync(id);
            });
    }
}