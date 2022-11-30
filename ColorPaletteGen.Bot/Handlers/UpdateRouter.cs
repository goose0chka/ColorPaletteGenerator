using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ColorPaletteGen.Bot.Handlers;

public class UpdateRouter : IUpdateHandler
{
    private readonly UpdateHandler _handler;

    public UpdateRouter(UpdateHandler handler)
    {
        _handler = handler;
    }

    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        return update.Type switch
        {
            UpdateType.Message => HandleMessage(botClient, update.Message!, cancellationToken),
            UpdateType.CallbackQuery => HandleCallbackQuery(botClient, update.CallbackQuery!, cancellationToken),
            _ => Task.CompletedTask
        };
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
        => _handler.HandlePollingErrorAsync(exception);

    private Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        return callbackQuery.Data switch
        {
            "refresh" => _handler.HandleRefreshCallback(botClient, callbackQuery, cancellationToken),
            _ => callbackQuery.Data!.Contains("lock")
                ? _handler.HandleColorLock(botClient, callbackQuery, cancellationToken)
                : Task.CompletedTask
        };
    }

    private Task HandleMessage(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        return message.Text switch
        {
            "/generate" => _handler.GenerateColorPalette(botClient, message, cancellationToken),
            _ => Task.CompletedTask
        };
    }
}
