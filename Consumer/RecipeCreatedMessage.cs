using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    public class RecipeCreatedMessage
    {
        public Guid RecipeId { get; set; }
        public string Title { get; set; } = default!;
        public string Cuisine { get; set; } = default!;
        public string Difficulty { get; set; } = default!;
        public Guid UserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
