using System.Globalization;
using ColorPaletteGen.Core;
using ColorPaletteGen.Core.Extensions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace ColorPaletteGen.Bot;

public class UpdateHandler : IUpdateHandler
{
    private static readonly InlineKeyboardMarkup Markup =
        new(InlineKeyboardButton.WithCallbackData("Refresh"));

    private readonly ILogger<UpdateHandler> _logger;
    private readonly Dictionary<ChatId, int> _messageIds = new();

    public UpdateHandler(ILogger<UpdateHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        return update.Type switch
        {
            UpdateType.Message => HandleMessage(botClient, update.Message!, cancellationToken),
            _ => Task.CompletedTask
        };
    }

    private async Task HandleMessage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
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
                newMessage = await botClient.EditMessageMediaAsync(
                    chatId, messageId, media,
                    replyMarkup: Markup,
                    cancellationToken: cancellationToken);
            }
            else
            {
                var media = new InputOnlineFile(stream);
                newMessage = await botClient.SendPhotoAsync(
                    message.Chat.Id, media,
                    replyMarkup: Markup,
                    cancellationToken: cancellationToken);
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
