using ColorPaletteGen.Bot;
using ColorPaletteGen.Core;
using ColorPaletteGen.Core.GenerationStrategies;
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
            .AddSingleton<IUpdateHandler, UpdateHandler>()
            .AddSingleton<IGenerationStrategy<GenerationStrategy>, RandomGenerationStrategy>()
            .AddSingleton<IGenerationStrategy<GenerationStrategy>, AnalogousGenerationStrategy>()
            .AddSingleton<ColorPaletteGenerator>();
    })
    .Build();

await host.RunAsync();
