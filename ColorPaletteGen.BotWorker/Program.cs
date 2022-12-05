using ColorPaletteGen.BotWorker;
using ColorPaletteGen.BotWorker.Handlers;
using ColorPaletteGen.Core;
using ColorPaletteGen.Core.GenerationStrategies;
using ColorPaletteGen.DAL.Context;
using ColorPaletteGen.DAL.Model;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder => builder.AddEnvironmentVariables())
    .ConfigureServices((context, collection) =>
    {
        var token = context.Configuration["BotApiKey"]!;
        var connStr = context.Configuration.GetConnectionString("BotDb");
        collection
            .AddHostedService<Worker>()
            .AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token))
            .AddSingleton<UpdateHandler>()
            .AddSingleton<IUpdateHandler, UpdateRouter>()
            .AddSingleton<IGenerationStrategy<GenerationStrategy>, RandomGenerationStrategy>()
            .AddSingleton<IGenerationStrategy<GenerationStrategy>, AnalogousGenerationStrategy>()
            .AddSingleton<ColorPaletteGenerator>()
            .AddDbContext<DataContext>(opt => opt.UseSqlite(connStr));
    })
    .Build();

await host.RunAsync();
