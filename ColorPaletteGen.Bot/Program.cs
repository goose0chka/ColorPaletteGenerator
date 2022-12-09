using ColorPaletteGen.Bot;
using ColorPaletteGen.Bot.Services;
using ColorPaletteGen.Core;
using ColorPaletteGen.Core.GenerationStrategies;
using ColorPaletteGen.DAL.Context;
using ColorPaletteGen.DAL.Model;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var botConfigSection = builder.Configuration.GetSection(BotConfiguration.ConfigurationSection);
builder.Services
    .Configure<BotConfiguration>(botConfigSection)
    .Configure<RouteOptions>(options => options.LowercaseUrls = true);
var config = botConfigSection.Get<BotConfiguration>();

var services = builder.Services;
services.AddControllers()
    .AddNewtonsoftJson();

services.AddHostedService<WebhookConfigurationService>()
    .AddScoped<UpdateHandlerService>()
    .AddScoped<ColorPaletteGenerator>()
    .AddSingleton<IGenerationStrategy<GenerationStrategy>, RandomGenerationStrategy>()
    .AddDbContext<DataContext>((_, optionsBuilder) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("BotDb");
        optionsBuilder.UseSqlite(connectionString);
    });

services.AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(config.Token, httpClient));

#if DEBUG
services.AddSwaggerGen();
#endif

var app = builder.Build();
app.MapControllers();

#if DEBUG
app.UseHttpLogging()
    .UseSwagger()
    .UseSwaggerUI();
#endif

app.Run();
