using Domain.DTOs;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecipeApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        // ✅ GET: api/ingredients
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ingredients = await _ingredientService.GetAllAsync();
            return Ok(ingredients);
        }

        // ✅ POST: api/ingredients
        // ⬅️ Այստեղ ընդունում ենք միայն Name (առանց Id)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateIngredientDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            var ingredient = await _ingredientService.CreateAsync(dto);

            // վերադարձնում ենք նոր ստեղծված Ingredient-ը (Id + Name)
            return Ok(ingredient);
        }

        // ✅ DELETE: api/ingredients/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _ingredientService.DeleteAsync(id);

            if (!success)
                return NotFound($"Ingredient with id '{id}' not found.");

            return NoContent(); // 204 — հաջողությամբ ջնջվեց
        }
    }
}
