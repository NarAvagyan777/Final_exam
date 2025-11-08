namespace Domain.DTOs
{
    public class RatingDto
    {
        public Guid RecipeId { get; set; }
        public Guid UserId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
    }
}
