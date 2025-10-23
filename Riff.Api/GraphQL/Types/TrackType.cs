using Application.Services.Interfaces;
using GraphQL.Types;
using Riff.Api.Contracts.Dto;

namespace Riff.Api.GraphQL.Types;

public sealed class TrackType : ObjectGraphType<TrackResponse>
{
    public TrackType()
    {
        Name = "Track";
        Description = "Represents a track in a room's playlist.";

        Field(t => t.Id, type: typeof(IdGraphType))
            .Description("The unique ID of the track.");
        
        Field(t => t.Title)
            .Description("The title of the track.");
        
        Field(t => t.Artist)
            .Description("The artist of the track.");
        
        Field(t => t.Url)
            .Description("The URL to the music source.");
        
        Field(t => t.DurationInSeconds, type: typeof(IntGraphType))
            .Description("Duration of the track in seconds.");

        Field<UserType>("addedBy")
            .Description("The user who added this track.")
            .ResolveAsync(async context =>
            {
                var userService = context.RequestServices!.GetRequiredService<IUserService>();
                return await userService.GetByIdAsync(context.Source.AddedById);
            });
    }
}