using System.Globalization;
using ColorPaletteGen.Core;
using ColorPaletteGen.Core.Extensions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace ColorPaletteGen.Bot;

public class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<UpdateHandler> _logger;
    private readonly Dictionary<ChatId, int> _messageIds = new();

    public UpdateHandler(ILogger<UpdateHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var message = update.Message;
        if (message?.EntityValues?.ElementAtOrDefault(0) == "/generate")
        {
            var chatId = message.Chat.Id;
            var palette = new ColorPalette();
            await using var stream = palette.GetImageStream(500, 200);
            Message newMessage;
            if (_messageIds.ContainsKey(chatId))
            {
                var messageId = _messageIds[chatId];
                var @base = new InputMedia(stream, "palette");
                var media = new InputMediaPhoto(@base);
                newMessage =
                    await botClient.EditMessageMediaAsync(chatId, messageId, media, cancellationToken: cancellationToken);
            }
            else
            {
                var media = new InputOnlineFile(stream);
                newMessage =
                    await botClient.SendPhotoAsync(message.Chat.Id, media, cancellationToken: cancellationToken);
            }
            
            _messageIds[message.Chat.Id] = newMessage.MessageId;
            await botClient.DeleteMessageAsync(chatId, message.MessageId, cancellationToken: cancellationToken);
        }
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var time = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        _logger.LogError(exception, "Polling error ({Time}): ", time);
        return Task.CompletedTask;
    }
}
