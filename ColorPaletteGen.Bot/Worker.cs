using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace ColorPaletteGen.Bot;

public class Worker : BackgroundService
{
    private static readonly ReceiverOptions Options = new()
    {
        AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
    };
    
    private readonly ITelegramBotClient _client;
    private readonly IUpdateHandler _handler;

    public Worker(ITelegramBotClient client, IUpdateHandler handler)
    {
        _client = client;
        _handler = handler;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.StartReceiving(_handler, Options, stoppingToken);
        return Task.CompletedTask;
    }
}
