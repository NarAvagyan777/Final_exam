using Domain.DTOs;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRecipeService
    {
        // ✅ Էջավորված և ֆիլտրված ցուցակ
        Task<PagedResult<RecipeDTO>> GetAllAsync(
            int page = 1,
            int pageSize = 10,
            string? cuisine = null,
            string? difficulty = null);

        // ✅ Մնացածը թող նույնը մնա
        Task<RecipeDTO?> GetByIdAsync(Guid id);
        Task<RecipeDTO> CreateAsync(CreateRecipeDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RateRecipeAsync(Guid recipeId, Guid userId, int score, string? comment);
        Task<RecipeDTO> UpdateAsync(Recipe recipe);
        Task<Recipe?> GetEntityByIdAsync(Guid id);
    }
}
