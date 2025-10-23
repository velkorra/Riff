using Riff.Api.Contracts.Dto;

namespace Application.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(RegisterUserRequest request);
    Task<UserResponse> GetByIdAsync(Guid id);
}