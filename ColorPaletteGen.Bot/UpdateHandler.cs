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
        new(InlineKeyboardButton.WithCallbackData("🔁", "refresh"));

    private readonly ILogger<UpdateHandler> _logger;

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
            UpdateType.CallbackQuery => HandleCallbackQuery(botClient, update.CallbackQuery!, cancellationToken),
            _ => Task.CompletedTask
        };
    }
    
    private static Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        return callbackQuery.Data switch
        {
            "refresh" => HandleRefreshCallback(botClient, callbackQuery, cancellationToken),
            _ => Task.CompletedTask
        };
    }

    private static Task HandleMessage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        return message.Text switch
        {
            "/generate" => GenerateColorPalette(botClient, message, cancellationToken),
            _ => Task.CompletedTask
        };
    }

    private static async Task GenerateColorPalette(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var palette = new ColorPalette();
        await using var stream = palette.GetImageStream(1000, 400);
        var media = new InputOnlineFile(stream);
        await botClient.SendPhotoAsync(
            message.Chat.Id, media,
            replyMarkup: Markup,
            cancellationToken: cancellationToken);
        await botClient.DeleteMessageAsync(chatId, message.MessageId, cancellationToken: cancellationToken);
    }

    private static async Task HandleRefreshCallback(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        
        var palette = new ColorPalette();
        await using var stream = palette.GetImageStream(1000, 400);
        
        var @base = new InputMedia(stream, "palette");
        var media = new InputMediaPhoto(@base);
        
        await botClient.EditMessageMediaAsync(
            chatId, messageId, media,
            replyMarkup: Markup,
            cancellationToken: cancellationToken);
    }
    
    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var time = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        _logger.LogError(exception, "Polling error ({Time}): ", time);
        return Task.CompletedTask;
    }
}
