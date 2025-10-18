using Riff.Api.Contracts.Dto;
using Riff.Infrastructure.Persistance.Entities;
using Riok.Mapperly.Abstractions;

namespace Riff.ApiGateway.Mappings;

[Mapper]
public static partial class RoomMapper
{
    [MapperIgnoreSource(nameof(Room.PasswordHash))]
    [MapperIgnoreSource(nameof(Room.Owner))]
    public static partial RoomResponse ToDto(this Room source);
}
