using Domain.DTOs;

namespace Domain.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> RegisterAsync(CreateUserDto dto, string password);
        Task<string?> LoginAsync(string username, string password);
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllAsync();
    }
}
