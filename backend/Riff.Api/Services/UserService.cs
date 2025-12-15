using Microsoft.EntityFrameworkCore;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Exceptions;
using Riff.Api.Mappings;
using Riff.Api.Services.Interfaces;
using Riff.Infrastructure;
using Riff.Infrastructure.Entities;

namespace Riff.Api.Services;

public class UserService(RiffContext context) : IUserService
{
    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await context.Users.FindAsync(id);
        if (user is null)
        {
            throw new ResourceNotFoundException(nameof(User), id);
        }

        return user.ToDto();
    }
}