using ColorPaletteGen.Core;
using ColorPaletteGen.Core.Extensions;
using ColorPaletteGen.DAL.Context;
using ColorPaletteGen.DAL.Model;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace ColorPaletteGen.Bot.Services;

public class UpdateHandlerService
{
    private readonly ITelegramBotClient _bot;
    private readonly DataContext _context;
    private readonly ColorPaletteGenerator _generator;

    private static readonly IEnumerable<InlineKeyboardButton> RefreshButton = new[]
        { InlineKeyboardButton.WithCallbackData("🔁", "refresh") };

    public UpdateHandlerService(ITelegramBotClient bot, DataContext context, ColorPaletteGenerator generator)
    {
        _bot = bot;
        _context = context;
        _generator = generator;
    }
    
    private static (long, int) GetId(Message message)
        => (message.Chat.Id, message.MessageId);
    
    private static InlineKeyboardMarkup GetKeyboard(ColorPalette palette)
    {
        var lockButtons = palette.Colors
            .Select((color, i) =>
            {
                var text = color.Locked ? "🔒" : "🔓";
                var data = $"lock{i}";
                return InlineKeyboardButton.WithCallbackData(text, data);
            });

        return palette.Colors.All(x => x.Locked) 
            ? new InlineKeyboardMarkup(lockButtons) 
            : new InlineKeyboardMarkup(new []{lockButtons, RefreshButton});
    }

    #region Routers

    public Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        return update switch
        {
            { Message: { } message} => HandleMessage(message, cancellationToken),
            { CallbackQuery: {} callbackQuery } => HandleCallbackQuery(callbackQuery, cancellationToken),
            _ => Task.CompletedTask
        };
    }
    
    private Task HandleMessage(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is not { } messageText)
        {
            return Task.CompletedTask;
        }
        return messageText.Split(' ')[0] switch
        {
            "/generate" => GenerateColorPalette(message, cancellationToken),
            _ => Task.CompletedTask
        };
    }

    private Task HandleCallbackQuery(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        return callbackQuery.Data switch
        {
            "refresh" => HandleRefreshCallback(callbackQuery, cancellationToken),
            _ => callbackQuery.Data!.Contains("lock")
                ? HandleColorLock(callbackQuery, cancellationToken)
                : Task.CompletedTask
        };
    }

    #endregion

    #region Handlers

    private async Task GenerateColorPalette(Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var palette = new ColorPalette
        {
            ChatId = chatId
        };
        _generator.Generate(palette);

        await using var stream = palette.GetImageStream(1000, 400);
        var media = new InputOnlineFile(stream);
        var sentMessage = await _bot.SendPhotoAsync(
            chatId, media,
            replyMarkup: GetKeyboard(palette),
            cancellationToken: cancellationToken);
        await _bot.DeleteMessageAsync(chatId, message.MessageId, cancellationToken);

        palette.Id = sentMessage.MessageId;
        
        _context.Palettes.Add(palette);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    private async Task HandleRefreshCallback(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var (chatId, messageId) = GetId(callbackQuery.Message!);
        
        var palette = await _context.Palettes.FindAsync(new object[] { chatId, messageId }, cancellationToken);
        if (palette is null || palette.Colors.All(color => color.Locked))
        {
            return;
        }

        _generator.Generate(palette);
        await _context.SaveChangesAsync(cancellationToken);

        await using var stream = palette.GetImageStream(1000, 400);
        var @base = new InputMedia(stream, "palette");
        var media = new InputMediaPhoto(@base);

        await _bot.EditMessageMediaAsync(
            chatId, messageId, media,
            GetKeyboard(palette),
            cancellationToken);
    }

    private async Task HandleColorLock(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var (chatId, messageId) = GetId(callbackQuery.Message!);
        
        var palette = await _context.Palettes.FindAsync(new object[] { chatId, messageId },
            cancellationToken: cancellationToken);
        if (palette is null)
        {
            return;
        }

        var colorIndexStr = new string(callbackQuery.Data!.Skip(4).ToArray());
        var colorIndex = int.Parse(colorIndexStr);
        palette.InvertLock(colorIndex);
        
        await _context.SaveChangesAsync(cancellationToken);
        await _bot.EditMessageReplyMarkupAsync(
            chatId, messageId,
            GetKeyboard(palette),
            cancellationToken);
    }
    
    #endregion
}
