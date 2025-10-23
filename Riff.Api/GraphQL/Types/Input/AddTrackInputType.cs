using GraphQL.Types;

namespace Riff.Api.GraphQL.Types.Input;

public sealed class AddTrackInputType : InputObjectGraphType
{
    public AddTrackInputType()
    {
        Name = "AddTrackInput";
        Description = "Input for adding a new track to a room.";

        Field<NonNullGraphType<StringGraphType>>("title");
        Field<NonNullGraphType<StringGraphType>>("artist");
        Field<NonNullGraphType<StringGraphType>>("url");
        Field<NonNullGraphType<IntGraphType>>("durationInSeconds");
    }
}