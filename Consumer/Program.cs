using MassTransit;
using Consumer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // ✅ Կարդում ենք RabbitMQ-ի hostname-ը (Docker-ում գալիս է docker-compose.yml-ից)
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";

        services.AddMassTransit(x =>
        {
            // 🧩 Ավելացնում ենք մեր Consumer-ը
            x.AddConsumer<RecipeCreatedConsumer>();

            // 🐇 Կոնֆիգուրացնում ենք RabbitMQ-ն
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(rabbitHost, "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // 🎧 Կոնֆիգուրացնում ենք Receive Endpoint-ը
                cfg.ReceiveEndpoint("recipe-created-queue", e =>
                {
                    e.ConfigureConsumer<RecipeCreatedConsumer>(ctx);
                });

                // ✅ Retry քաղաքականություն (եթե RabbitMQ-ն fail է լինում)
                cfg.UseMessageRetry(r =>
                {
                    r.Interval(5, TimeSpan.FromSeconds(5)); // 5 փորձ՝ ամեն 5 վայրկյան
                });

                // ✅ Redelivery քաղաքականություն
                cfg.UseDelayedRedelivery(r =>
                {
                    r.Intervals(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(30)
                    );
                });

                // ✅ Console հաղորդագրություն, երբ հաջողությամբ միանում է RabbitMQ-ին
                cfg.ConfigureEndpoints(ctx);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"✅ Connected to RabbitMQ at '{rabbitHost}'");
                Console.WriteLine($"📩 Listening on queue: recipe-created-queue\n");
                Console.ResetColor();
            });
        });

        // 👇 MassTransit Host Service-ը ավտոմատ աշխատելու համար
        services.AddMassTransitHostedService(true);
    })
    .Build();

await host.RunAsync();
