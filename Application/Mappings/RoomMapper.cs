using Riff.Api.Contracts.Dto;
using Riff.Infrastructure.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Mappings;

[Mapper]
public static partial class RoomMapper
{
    [MapperIgnoreSource(nameof(Room.PasswordHash))]
    [MapperIgnoreSource(nameof(Room.Owner))]
    [MapperIgnoreSource(nameof(Room.Playlist))]
    [MapperIgnoreTarget(nameof(RoomResponse.Links))]
    public static partial RoomResponse ToDto(this Room source);
    
    public static partial IEnumerable<RoomResponse> ToDto(this IEnumerable<Room> source);
    
}