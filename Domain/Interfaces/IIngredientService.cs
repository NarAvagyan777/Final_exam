using Domain.DTOs;

namespace Domain.Interfaces
{
    public interface IIngredientService
    {
        // ✅ Բոլոր ингредиենտները վերադարձնում է (Id + Name)
        Task<IEnumerable<IngridientDto>> GetAllAsync();

        // ✅ Ստեղծում է նոր ингредиենտ (ընդունում է միայն Name)
        Task<IngridientDto> CreateAsync(CreateIngredientDto dto);

        // ✅ Ջնջում է ըստ Guid Id
        Task<bool> DeleteAsync(Guid id);
    }
}
