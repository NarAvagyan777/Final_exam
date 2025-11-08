using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CreateRecipeDto
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Cuisine { get; set; } = default!;
        public string Difficulty { get; set; } = default!;

        // Սա կարող ես թողնել կամ հանել՝ ըստ ցանկության
       // public double AverageRating { get; set; } = 0;

        // ✅ UserId պետք է, որ իմանանք ով է ստեղծել recipe-ն
        public Guid UserId { get; set; }
    }
}
