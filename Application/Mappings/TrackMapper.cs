using Riff.Api.Contracts.Dto;
using Riff.Infrastructure.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Mappings;

[Mapper]
public static partial class TrackMapper
{
    [MapperIgnoreSource(nameof(Track.Room))]
    [MapperIgnoreSource(nameof(Track.AddedBy))]
    [MapperIgnoreTarget(nameof(TrackResponse.Links))]
    public static partial TrackResponse ToDto(this Track source);

    public static partial IEnumerable<TrackResponse> ToDto(this IEnumerable<Track> source);
}