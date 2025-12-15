using System.Diagnostics.CodeAnalysis;
using Riff.Api.Contracts.Dto;
using Riff.Infrastructure.Entities;
using Riok.Mapperly.Abstractions;

namespace Riff.Api.Mappings;

[Mapper]
[SuppressMessage("Mapper", "RMG020:Source member is not mapped to any target member")]
[SuppressMessage("Mapper", "RMG012:Source member was not found for target member")]
public static partial class RoomMapper
{
    public static partial RoomResponse ToDto(this Room source);

    public static partial IEnumerable<RoomResponse> ToDto(this IEnumerable<Room> source);
}