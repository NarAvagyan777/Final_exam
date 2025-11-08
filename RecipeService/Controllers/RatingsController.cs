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

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // ✅ GET api/ratings/recipe/{recipeId}
        [HttpGet("recipe/{recipeId}")]
        public async Task<IActionResult> GetByRecipeId(Guid recipeId)
        {
            var ratings = await _ratingService.GetByRecipeIdAsync(recipeId);
            return Ok(ratings);
        }

        // ✅ DELETE api/ratings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _ratingService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
