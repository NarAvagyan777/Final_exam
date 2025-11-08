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

       
        public Guid UserId { get; set; }
    }
}
