using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Infrastructure.Messaging
{
    public class RabbitMqPublisher : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqPublisher()
        {
            // ✅ Կարդում ենք host-ը միջավայրից (Docker-ում կգա RABBITMQ_HOST)
            var hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";

            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = "guest",
                Password = "guest"
            };

            // ✅ Ավելացնում ենք retry logic՝ եթե RabbitMQ-ն դեռ չի պատրաստ
            const int maxRetries = 10;
            int retryCount = 0;

            while (true)
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    // ✅ Հռչակում ենք exchange (եթե չկա՝ ստեղծում է)
                    _channel.ExchangeDeclare(
                        exchange: "recipe_exchange",
                        type: ExchangeType.Fanout,
                        durable: true,
                        autoDelete: false,
                        arguments: null
                    );

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✅ Connected to RabbitMQ at '{hostName}'");
                    Console.ResetColor();
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠️ Failed to connect to RabbitMQ (attempt {retryCount}/{maxRetries}): {ex.Message}");
                    Console.ResetColor();

                    if (retryCount >= maxRetries)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Could not connect to RabbitMQ after several attempts. Exiting...");
                        Console.ResetColor();
                        throw;
                    }

                    Thread.Sleep(3000); // ⏳ սպասում ենք 3 վայրկյան նորից փորձելուց առաջ
                }
            }
        }

        // ✅ Publish մեթոդը՝ հաղորդագրություն ուղարկելու համար
        public void Publish<T>(T message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = _channel.CreateBasicProperties();
            props.Persistent = true; // պահպանում է հաղորդագրությունը queue-ում, եթե RabbitMQ-ն վերագործարկվի

            _channel.BasicPublish(
                exchange: "recipe_exchange",
                routingKey: "",
                basicProperties: props,
                body: body
            );

            Console.WriteLine($"📤 Հաղորդագրությունը ուղարկվեց RabbitMQ-ին: {json}");
        }

        // ✅ Փակում ենք ռեսուրսները ճիշտ ձևով
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
