using Riff.Api.Contracts.Dto;

namespace Riff.Api.Services.Interfaces;

public interface ITrackService
{
    Task<IEnumerable<TrackResponse>> GetPlaylistAsync(Guid roomId);
    Task<TrackResponse> GetByIdAsync(Guid trackId);
    Task<IEnumerable<TrackResponse>> GetGlobalTopAsync(int limit = 20);
}