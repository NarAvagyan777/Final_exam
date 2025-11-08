using Domain.DTOs;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecipeApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NutritionsController : ControllerBase
    {
        private readonly INutritionService _nutritionService;

        public NutritionsController(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        // ✅ GET api/nutritions/recipe/{recipeId}
        [HttpGet("recipe/{recipeId}")]
        public async Task<IActionResult> GetByRecipeId(Guid recipeId)
        {
            var nutrition = await _nutritionService.GetByRecipeIdAsync(recipeId);
            if (nutrition == null)
                return NotFound(new { message = "Nutrition not found for this recipe." });

            return Ok(nutrition);
        }

        // ✅ POST api/nutritions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNutritionDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid nutrition data." });

            var created = await _nutritionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByRecipeId), new { recipeId = created.RecipeId }, created);
        }

        // ✅ PUT api/nutritions
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NutritionDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid nutrition data." });

            var success = await _nutritionService.UpdateAsync(dto);
            if (!success)
                return NotFound(new { message = "Nutrition not found to update." });

            return Ok(new { message = "Nutrition updated successfully." });
        }
    }
}
