namespace Domain.Entities
{
    public class Recipe
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Cuisine { get; set; } = default!;
        public string Difficulty { get; set; } = default!;
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public Nutrition Nutrition { get; set; } = default!;
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public double AverageRating => Ratings.Any() ? Ratings.Average(r => r.Score) : 0;
    }
}
