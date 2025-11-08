using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CreateRatingDto
    {

        public Guid RecipeId { get; set; }
        public Guid UserId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
    }
}
