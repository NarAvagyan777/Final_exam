using Domain.DTOs;

namespace Domain.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<RecipeDTO>> GetAllAsync(int page = 1, int pageSize = 10, string? cuisine = null, string? difficulty = null);
        Task<RecipeDTO?> GetByIdAsync(Guid id);
        Task<RecipeDTO> CreateAsync(CreateRecipeDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RateRecipeAsync(Guid recipeId, Guid userId, int score, string? comment);
    }
}
