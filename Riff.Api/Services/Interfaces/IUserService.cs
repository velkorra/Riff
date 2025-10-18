using Riff.Api.Contracts.Dto;

namespace Riff.Api.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse> Create(RegisterUserRequest request);
}
