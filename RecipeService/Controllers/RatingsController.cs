using Domain.DTOs;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecipeApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly IRecipeService _recipeService;

        public RatingsController(IRatingService ratingService, IRecipeService recipeService)
        {
            _ratingService = ratingService;
            _recipeService = recipeService;
        }

        // ✅ GET api/ratings/recipe/{recipeId}
        [HttpGet("recipe/{recipeId}")]
        public async Task<IActionResult> GetRatingsByRecipe(Guid recipeId)
        {
            var recipe = await _recipeService.GetByIdAsync(recipeId);
            if (recipe == null)
                return NotFound(new { message = "Recipe not found." });

            var ratings = await _ratingService.GetByRecipeIdAsync(recipeId);
            return Ok(ratings);
        }

        // ✅ POST api/ratings
        [HttpPost]
        public async Task<IActionResult> AddRating([FromBody] CreateRatingDto dto)
        {
            if (dto == null)
                return BadRequest("Rating data is required.");

            if (dto.Score < 1 || dto.Score > 5)
                return BadRequest("Score must be between 1 and 5.");

            var recipe = await _recipeService.GetEntityByIdAsync(dto.RecipeId);
            if (recipe == null)
                return NotFound(new { message = "Recipe not found." });

            var result = await _ratingService.CreateAsync(dto);
            if (result == null)
                return BadRequest(new { message = "Failed to add rating." });

            return Ok(new
            {
                message = "✅ Rating added successfully.",
                rating = result
            });
        }

        // ✅ PUT api/ratings/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRatingDto dto)
        {
            if (dto == null)
                return BadRequest("Rating data is required.");

            var updated = await _ratingService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = "Rating not found." });

            return Ok(new
            {
                message = "✅ Rating updated successfully.",
                rating = updated
            });
        }

        // ✅ DELETE api/ratings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _ratingService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Rating not found." });

            return Ok(new { message = "✅ Rating deleted successfully." });
        }
    }
}
