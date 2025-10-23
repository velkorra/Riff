using Riff.Api.Contracts.Dto;
using Riff.Infrastructure.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Mappings;

[Mapper]
public static partial class UserMapper
{
    [MapperIgnoreSource(nameof(User.PasswordHash))]
    [MapperIgnoreSource(nameof(User.OwnedRooms))]
    [MapperIgnoreSource(nameof(User.AddedTracks))]
    [MapperIgnoreTarget(nameof(UserResponse.Links))]
    public static partial UserResponse ToDto(this User source);
}