using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace RecipeApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? cuisine = null,
            [FromQuery] string? difficulty = null)
        {
            var recipes = await _recipeService.GetAllAsync(page, pageSize, cuisine, difficulty);
            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var recipe = await _recipeService.GetByIdAsync(id);
            if (recipe == null)
                return NotFound(new { message = "Recipe not found." });

            return Ok(recipe);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRecipeDto dto)
        {
            if (dto == null)
                return BadRequest("Recipe data is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var recipe = await _recipeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _recipeService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Recipe not found." });

            return NoContent();
        }

        [HttpPost("{id}/rate")]
        public async Task<IActionResult> Rate(
            Guid id,
            [FromQuery] Guid userId,
            [FromQuery] int score,
            [FromQuery] string? comment)
        {
            if (score < 1 || score > 5)
                return BadRequest("Score must be between 1 and 5.");

            var success = await _recipeService.RateRecipeAsync(id, userId, score, comment);
            if (!success)
                return BadRequest("Failed to rate recipe. Recipe may not exist.");

            return Ok(new { message = "Recipe rated successfully." });
        }

        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] CreateUploadImage dto)
        {
            var recipeEntity = await _recipeService.GetEntityByIdAsync(dto.RecipeId);
            if (recipeEntity == null)
                return NotFound(new { message = "Recipe not found." });

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            recipeEntity.ImagePath = $"/images/{fileName}";
            await _recipeService.UpdateAsync(recipeEntity);

            return Ok(new
            {
                message = "✅ Նկարը հաջողությամբ վերբեռնվեց։",
                imageUrl = recipeEntity.ImagePath
            });
        }
    }
}
