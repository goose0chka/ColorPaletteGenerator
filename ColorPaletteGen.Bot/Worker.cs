using Telegram.Bot;
using Telegram.Bot.Polling;

namespace ColorPaletteGen.Bot;

public class Worker : BackgroundService
{
    private readonly ITelegramBotClient _client;
    private readonly IUpdateHandler _handler;

    public Worker(ITelegramBotClient client, IUpdateHandler handler)
    {
        _client = client;
        _handler = handler;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.StartReceiving(_handler, null, stoppingToken);
        return Task.CompletedTask;
    }
}
