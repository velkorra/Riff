using Riff.Api.Contracts.Dto;

namespace Application.Services.Interfaces;

public interface ITrackService
{
    Task<TrackResponse> AddTrackAsync(Guid roomId, AddTrackRequest request, Guid userId);
    Task<IEnumerable<TrackResponse>> GetPlaylistAsync(Guid roomId);
    Task DeleteTrackAsync(Guid trackId, Guid userId);
}