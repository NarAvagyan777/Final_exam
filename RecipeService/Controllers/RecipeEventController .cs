using Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;
using System;

namespace RecipeApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeEventController : ControllerBase
    {
        private readonly RabbitMqPublisher _publisher;

        public RecipeEventController(RabbitMqPublisher publisher)
        {
            _publisher = publisher;
        }

        // ✅ POST api/recipeevent/send
        [HttpPost("send")]
        public IActionResult SendTestEvent()
        {
            // Ստեղծում ենք event object
            var message = new
            {
                Event = "RecipeCreated",
                RecipeId = Guid.NewGuid(),
                Title = "Test Recipe from Controller",
                Cuisine = "Italian",
                Difficulty = "Medium",
                UserId = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow
            };

            // Ուղարկում ենք RabbitMQ-ին
            _publisher.Publish(message);

            return Ok(new
            {
                success = true,
                message = "📤 Event successfully sent to RabbitMQ!",
                data = message
            });
        }
    }
}
