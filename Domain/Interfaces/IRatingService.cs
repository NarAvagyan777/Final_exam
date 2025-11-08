using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRatingService
    {
        Task<IEnumerable<RatingDto>> GetByRecipeIdAsync(Guid recipeId);
        Task<RatingDto> AddRatingAsync(RatingDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
