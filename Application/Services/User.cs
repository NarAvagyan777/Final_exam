using System;
using System.Security.Cryptography;
using System.Text;
using Domain.Entities;
using Domain.DTOs;
using Domain.Interfaces;
using Infrastructure.RepositoryInterfaces;
using Infrastructure.Auth;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtService _jwtService;

        public UserService(IUserRepository userRepo, JwtService jwtService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
        }

        // ======================================
        // ✅ RegisterAsync (CreateUserDto version)
        // ======================================
        public async Task<UserDto?> RegisterAsync(CreateUserDto dto, string password)
        {
            // Ստուգում ենք՝ արդյոք user-ը արդեն կա email-ով
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                return null;

            // Ստեղծում ենք նոր user entity
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = Hash(password)
            };

            await _userRepo.AddAsync(user);

            // Վերադարձնում ենք DTO՝ առանց password
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }

        // ======================================
        // ✅ LoginAsync
        // ======================================
        public async Task<string?> LoginAsync(string username, string password)
        {
            var users = await _userRepo.FindAsync(u => u.Username == username);
            var user = users.FirstOrDefault();

            if (user == null || user.PasswordHash != Hash(password))
                return null;

            return _jwtService.GenerateToken(user);
        }

        // ======================================
        // ✅ GetByIdAsync
        // ======================================
        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }

        // ======================================
        // ✅ GetAllAsync
        // ======================================
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepo.GetAllAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            });
        }

        // ======================================
        // ✅ Password Hash helper
        // ======================================
        private static string Hash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
