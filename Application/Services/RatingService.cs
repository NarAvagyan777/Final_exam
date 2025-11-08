using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Domain.RepositoryInterfaces;
using Infrastructure.RepositoryInterfaces;

namespace Application.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepo;
        private readonly IRecipeRepository _recipeRepo;
        private readonly IUnitOfWork _unitOfWork;

        public RatingService(
            IRatingRepository ratingRepo,
            IRecipeRepository recipeRepo,
            IUnitOfWork unitOfWork)
        {
            _ratingRepo = ratingRepo;
            _recipeRepo = recipeRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RatingDto>> GetByRecipeIdAsync(Guid recipeId)
        {
            var ratings = await _ratingRepo.GetByRecipeIdAsync(recipeId);

            return ratings.Select(r => new RatingDto
            {
                Id = r.Id,
                RecipeId = r.RecipeId,
                UserId = r.UserId,
                Score = r.Score,
                Comment = r.Comment
            });
        }

        public async Task<RatingDto?> CreateAsync(CreateRatingDto dto)
        {
            Rating? newRating = null;

            await _unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                // ⬇️ Ստեղծում ենք նոր գնահատական
                newRating = new Rating
                {
                    RecipeId = dto.RecipeId,
                    UserId = dto.UserId,
                    Score = dto.Score,
                    Comment = dto.Comment
                };

                await _ratingRepo.AddAsync(newRating);

                // 📈 Թարմացնում ենք recipe-ի միջին գնահատականը
                var allRatings = await _ratingRepo.GetByRecipeIdAsync(dto.RecipeId);
                var recipe = await _recipeRepo.GetByIdAsync(dto.RecipeId);

                if (recipe != null)
                {
                    recipe.Ratings = allRatings.ToList();
                    recipe.UpdatedAt = DateTime.UtcNow;
                    await _recipeRepo.UpdateAsync(recipe);
                }
            });

            return newRating == null
                ? null
                : new RatingDto
                {
                 //   Id = newRating.Id,
                    RecipeId = newRating.RecipeId,
                    UserId = newRating.UserId,
                    Score = newRating.Score,
                    Comment = newRating.Comment
                };
        }

        // ✅ Թարմացնել գնահատականը
        public async Task<RatingDto?> UpdateAsync(Guid id, UpdateRatingDto dto)
        {
            var rating = await _ratingRepo.GetByIdAsync(id);
            if (rating == null) return null;

            await _unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                rating.Score = dto.Score;
                rating.Comment = dto.Comment;
                await _ratingRepo.UpdateAsync(rating);

                // 📈 Թարմացնում ենք միջին գնահատականը
                var allRatings = await _ratingRepo.GetByRecipeIdAsync(rating.RecipeId);
                var recipe = await _recipeRepo.GetByIdAsync(rating.RecipeId);

                if (recipe != null)
                {
                    recipe.Ratings = allRatings.ToList();
                    recipe.UpdatedAt = DateTime.UtcNow;
                    await _recipeRepo.UpdateAsync(recipe);
                }
            });

            return new RatingDto
            {
               // Id = rating.Id,
                RecipeId = rating.RecipeId,
                UserId = rating.UserId,
                Score = rating.Score,
                Comment = rating.Comment
            };
        }

        // ✅ Ջնջել գնահատականը
        public async Task<bool> DeleteAsync(Guid id)
        {
            var rating = await _ratingRepo.GetByIdAsync(id);
            if (rating == null) return false;

            await _unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                await _ratingRepo.DeleteAsync(rating);

                // 📉 Թարմացնում ենք միջին գնահատականը
                var allRatings = await _ratingRepo.GetByRecipeIdAsync(rating.RecipeId);
                var recipe = await _recipeRepo.GetByIdAsync(rating.RecipeId);

                if (recipe != null)
                {
                    recipe.Ratings = allRatings.ToList();
                    recipe.UpdatedAt = DateTime.UtcNow;
                    await _recipeRepo.UpdateAsync(recipe);
                }
            });

            return true;
        }
    }
}
