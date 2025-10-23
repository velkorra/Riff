using Application.Mappings;
using Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Riff.Api.Contracts.Dto;
using Riff.Api.Contracts.Exceptions;
using Riff.Infrastructure;
using Riff.Infrastructure.Entities;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly RiffContext _context;

    public UserService(RiffContext context)
    {
        _context = context;
    }

    public async Task<UserResponse> RegisterAsync(RegisterUserRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            throw new InvalidOperationException("A user with this username already exists.");
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

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            throw new ResourceNotFoundException(nameof(User), id);
        }
        return user.ToDto();
    }
}