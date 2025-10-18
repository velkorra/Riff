using Riff.Api.Contracts.Dto;
using Riff.Infrastructure.Persistance.Entities;
using Riok.Mapperly.Abstractions;

namespace Riff.Api.Mappings;

[Mapper]
public static partial class UserMapper
{
    [MapperIgnoreSource(nameof(User.PasswordHash))]
    public static partial UserResponse ToDto(this User source);
}
