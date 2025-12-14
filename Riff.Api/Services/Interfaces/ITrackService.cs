using Riff.Api.Contracts.Dto;

namespace Riff.Api.Services.Interfaces;

public interface ITrackService
{
    Task<TrackResponse> AddTrackAsync(Guid roomId, AddTrackRequest request, Guid userId);
    Task<IEnumerable<TrackResponse>> GetPlaylistAsync(Guid roomId);
    Task DeleteTrackAsync(Guid trackId, Guid userId);
    Task<TrackResponse> GetByIdAsync(Guid trackId);
    Task<IEnumerable<TrackResponse>> GetGlobalTopAsync(int limit = 20);
}