using ColorPaletteGen.Bot;
using ColorPaletteGen.Bot.Handlers;
using ColorPaletteGen.Core;
using ColorPaletteGen.Core.GenerationStrategies;
using ColorPaletteGen.DAL.Context;
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
            .AddSingleton<UpdateHandler>()
            .AddSingleton<IUpdateHandler, UpdateRouter>()
            .AddSingleton<IGenerationStrategy<GenerationStrategy>, RandomGenerationStrategy>()
            .AddSingleton<IGenerationStrategy<GenerationStrategy>, AnalogousGenerationStrategy>()
            .AddSingleton<ColorPaletteGenerator>()
            .AddDbContext<DataContext>();
    })
    .Build();

await host.RunAsync();
