using ColorPaletteGen.Bot;
using Telegram.Bot;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder => builder.AddEnvironmentVariables())
    .ConfigureServices((context, collection) =>
    {
        var token = context.Configuration["BotApiKey"];
        collection
            .AddHostedService<Worker>()
            .AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));
    })
    .Build();

await host.RunAsync();
