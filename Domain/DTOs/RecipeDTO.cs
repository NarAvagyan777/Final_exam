namespace Domain.DTOs
{
    public class RecipeDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Cuisine { get; set; } = default!;
        public string Difficulty { get; set; } = default!;
        public double AverageRating { get; set; }
        public Guid UserId { get; set; }
    }
}
