using MassTransit;

namespace Consumer
{
    public class RecipeCreatedConsumer : IConsumer<RecipeCreatedMessage>
    {
        public Task Consume(ConsumeContext<RecipeCreatedMessage> context)
        {
            var msg = context.Message;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("📩 Ստացվեց նոր Recipe event!");
            Console.ResetColor();

            Console.WriteLine($"➡ RecipeId: {msg.RecipeId}");
            Console.WriteLine($"➡ Title: {msg.Title}");
            Console.WriteLine($"➡ Cuisine: {msg.Cuisine}");
            Console.WriteLine($"➡ Difficulty: {msg.Difficulty}");
            Console.WriteLine($"➡ UserId: {msg.UserId}");
            Console.WriteLine($"➡ CreatedAt: {msg.CreatedAtUtc}");
            Console.WriteLine("✅ Հաղորդագրությունը հաջողությամբ մշակվեց!\n");

            return Task.CompletedTask;
        }
    }
}
