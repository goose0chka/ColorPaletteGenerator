using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace ColorPaletteGen.Bot.Services;

public class WebhookConfigurationService : IHostedService
{
    private readonly ITelegramBotClient _bot;
    private readonly ILogger<WebhookConfigurationService> _logger;
    private readonly BotConfiguration _config;

    public WebhookConfigurationService(ITelegramBotClient bot, 
        ILogger<WebhookConfigurationService> logger, 
        IOptions<BotConfiguration> config)
    {
        _bot = bot;
        _logger = logger;
        _config = config.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        
        var url = $"{_config.Host}/{_config.Endpoint}";
        _logger.LogInformation("Setting webhook: {Url}", url);
        await _bot.SetWebhookAsync(url, cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing webhook");
        await _bot.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}
