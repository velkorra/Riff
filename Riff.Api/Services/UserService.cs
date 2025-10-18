using Microsoft.EntityFrameworkCore;
using Riff.Api.Contracts.Dto;
using Riff.Api.Mappings;
using Riff.Api.Services.Interfaces;
using Riff.ApiGateway.Mappings;
using Riff.Infrastructure.Persistance;
using Riff.Infrastructure.Persistance.Entities;

namespace Riff.ApiGateway.Services;

public class UserService : IUserService
{
    private readonly RiffContext _context;

    public UserService(RiffContext context)
    {
        _context = context;
    }

    public async Task<UserResponse> Create(RegisterUserRequest request)
    {
        var isUsernameTaken = await _context.Users.AnyAsync(u => u.Username == request.Username);
        if (isUsernameTaken)
        {
            throw new InvalidOperationException("Username is already taken.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user.ToDto();
    }
}