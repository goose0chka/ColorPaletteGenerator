using ColorPaletteGen.Bot;
using Telegram.Bot;
using Telegram.Bot.Polling;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder => builder.AddEnvironmentVariables())
    .ConfigureServices((context, collection) =>
    {
        var token = context.Configuration["BotApiKey"];
        collection
            .AddHostedService<Worker>()
            .AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token))
            .AddSingleton<IUpdateHandler, UpdateHandler>();
    })
    .Build();

await host.RunAsync();
