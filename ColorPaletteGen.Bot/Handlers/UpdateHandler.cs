using System.Globalization;
using ColorPaletteGen.Core;
using ColorPaletteGen.Core.Extensions;
using ColorPaletteGen.DAL.Model;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace ColorPaletteGen.Bot.Handlers;

public class UpdateHandler
{
    private readonly ColorPaletteGenerator _generator;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly Dictionary<ChatId, ColorPalette> _palettes = new();

    public UpdateHandler(ILogger<UpdateHandler> logger, ColorPaletteGenerator generator)
    {
        _logger = logger;
        _generator = generator;
    }

    public Task HandlePollingErrorAsync(Exception exception)
    {
        var time = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        _logger.LogError(exception, "{Time}: ", time);
        return Task.CompletedTask;
    }

    internal async Task HandleColorLock(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        var message = callbackQuery.Message;
        var chatId = message!.Chat.Id;
        if (!_palettes.TryGetValue(chatId, out var palette))
        {
            return;
        }

        var colorIndexStr = new string(callbackQuery.Data!.Skip(4).ToArray());
        var colorIndex = int.Parse(colorIndexStr);
        palette.InvertLock(colorIndex);
        await botClient.EditMessageReplyMarkupAsync(
            chatId, message.MessageId,
            GetKeyboard(palette),
            cancellationToken);
    }

    private static InlineKeyboardMarkup GetKeyboard(ColorPalette palette)
    {
        var buttons = new List<IEnumerable<InlineKeyboardButton>>();
        var keyboardButtons = palette.Colors
            .Select((color, i) =>
            {
                var text = color.Locked ? "🔒" : "🔓";
                var data = $"lock{i}";
                return InlineKeyboardButton.WithCallbackData(text, data);
            });
        buttons.Add(keyboardButtons);

        if (palette.Colors.All(x => x.Locked))
        {
            return new InlineKeyboardMarkup(buttons);
        }

        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("🔁", "refresh") });
        return new InlineKeyboardMarkup(buttons);
    }

    internal async Task GenerateColorPalette(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        if (!_palettes.ContainsKey(chatId))
        {
            _palettes[chatId] = new ColorPalette();
        }

        var palette = _palettes[chatId];
        _generator.Generate(palette);

        await using var stream = palette.GetImageStream(1000, 400);
        var media = new InputOnlineFile(stream);
        await botClient.SendPhotoAsync(
            message.Chat.Id, media,
            replyMarkup: GetKeyboard(palette),
            cancellationToken: cancellationToken);
        await botClient.DeleteMessageAsync(chatId, message.MessageId, cancellationToken);
    }

    internal async Task HandleRefreshCallback(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;

        if (!_palettes.ContainsKey(chatId))
        {
            _palettes[chatId] = new ColorPalette();
        }

        var palette = _palettes[chatId];
        if (palette.Colors.All(color => color.Locked))
        {
            return;
        }

        _generator.Generate(palette);

        await using var stream = palette.GetImageStream(1000, 400);
        var @base = new InputMedia(stream, "palette");
        var media = new InputMediaPhoto(@base);

        await botClient.EditMessageMediaAsync(
            chatId, messageId, media,
            GetKeyboard(palette),
            cancellationToken);
    }
}
