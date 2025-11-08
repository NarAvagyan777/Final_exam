namespace Domain.Entities
{
    public class RecipeIngredient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = default!;

        public Guid IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = default!;

        public string Quantity { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
