using Domain.DTOs;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.RepositoryImplementations;
using Infrastructure.RepositoryInterfaces;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services
{
    public class RatingService
     : IRatingService
    {
        private readonly IRatingRepository _ratingRepo;

        public RatingService(IRatingRepository ratingRepo)
        {
            _ratingRepo = ratingRepo;
        }

        public async Task<IEnumerable<RatingDto>> GetByRecipeIdAsync(Guid recipeId)
        {
            var ratings = await _ratingRepo.GetByRecipeIdAsync(recipeId);
            return ratings.Select(r => new RatingDto
            {
                RecipeId = r.RecipeId,
                UserId = r.UserId,
                Score = r.Score,
                Comment = r.Comment
            });
        }

        public async Task<RatingDto> AddRatingAsync(RatingDto dto)
        {
            var entity = new Rating
            {
                RecipeId = dto.RecipeId,
                UserId = dto.UserId,
                Score = dto.Score,
                Comment = dto.Comment
            };
            await _ratingRepo.AddAsync(entity);
            return dto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _ratingRepo.GetByIdAsync(id);
            if (entity == null) return false;

            await _ratingRepo.DeleteAsync(entity);
            return true;
        }

    }
}
