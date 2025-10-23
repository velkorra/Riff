using GraphQL.Types;

namespace Riff.Api.GraphQL.Types.Input;

public sealed class RegisterUserInputType : InputObjectGraphType
{
    public RegisterUserInputType()
    {
        Name = "RegisterUserInput";
        Description = "Input for registering a new user.";
        
        Field<NonNullGraphType<StringGraphType>>("username");
        Field<NonNullGraphType<StringGraphType>>("password");
    }
}