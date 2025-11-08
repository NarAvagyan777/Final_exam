namespace Domain.Entities
{
    public class Rating
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Score { get; set; }
        public string? Comment { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
