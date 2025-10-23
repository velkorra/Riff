using GraphQL.Types;

namespace Riff.Api.GraphQL.Types.Input;

public sealed class CreateRoomInputType : InputObjectGraphType
{
    public CreateRoomInputType()
    {
        Name = "CreateRoomInput";
        Description = "Input for creating a new room.";

        Field<NonNullGraphType<StringGraphType>>("name");
        Field<StringGraphType>("password");
    }
}