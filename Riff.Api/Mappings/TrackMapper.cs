using System.Diagnostics.CodeAnalysis;
using Riff.Api.Contracts.Dto;
using Riff.Infrastructure.Entities;
using Riok.Mapperly.Abstractions;

namespace Riff.Api.Mappings;

[Mapper]
[SuppressMessage("Mapper", "RMG020:Source member is not mapped to any target member")]
[SuppressMessage("Mapper", "RMG012:Source member was not found for target member")]
public static partial class TrackMapper
{
    public static partial TrackResponse ToDto(this Track source);

    public static partial IEnumerable<TrackResponse> ToDto(this IEnumerable<Track> source);
}