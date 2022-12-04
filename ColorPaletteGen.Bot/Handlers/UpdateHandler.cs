using System.Globalization;
using ColorPaletteGen.Core;
using ColorPaletteGen.Core.Extensions;
using ColorPaletteGen.DAL.Context;
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
    private readonly IServiceProvider _provider;

    private AsyncServiceScope GetScope()
        => _provider.CreateAsyncScope();

    private static DataContext GetContext(AsyncServiceScope scope)
        => scope.ServiceProvider.GetService<DataContext>()!;

    public UpdateHandler(ILogger<UpdateHandler> logger, ColorPaletteGenerator generator, IServiceProvider provider)
    {
        _logger = logger;
        _generator = generator;
        _provider = provider;
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
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;
        var messageId = message.MessageId;

        await using var scope = GetScope();
        await using var context = GetContext(scope);
        var palette = await context.Palettes.FindAsync(new object[] { chatId, messageId },
            cancellationToken: cancellationToken);
        if (palette is null)
        {
            return;
        }

        var colorIndexStr = new string(callbackQuery.Data!.Skip(4).ToArray());
        var colorIndex = int.Parse(colorIndexStr);
        palette.InvertLock(colorIndex);

        context.Update(palette);
        await context.SaveChangesAsync(cancellationToken);

        await botClient.EditMessageReplyMarkupAsync(
            chatId, messageId,
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
        var palette = new ColorPalette
        {
            ChatId = chatId
        };
        _generator.Generate(palette);

        await using var stream = palette.GetImageStream(1000, 400);
        var media = new InputOnlineFile(stream);
        var sentMessage = await botClient.SendPhotoAsync(
            message.Chat.Id, media,
            replyMarkup: GetKeyboard(palette),
            cancellationToken: cancellationToken);
        await botClient.DeleteMessageAsync(chatId, message.MessageId, cancellationToken);

        palette.Id = sentMessage.MessageId;

        await using var scope = _provider.CreateAsyncScope();
        await using var dataContext = scope.ServiceProvider.GetService<DataContext>()!;
        dataContext.Palettes.Add(palette);
        await dataContext.SaveChangesAsync(cancellationToken);
    }

    internal async Task HandleRefreshCallback(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        
        await using var scope = GetScope();
        await using var context = GetContext(scope);
        var palette = await context.Palettes.FindAsync(new object[] { chatId, messageId }, cancellationToken);
        if (palette is null || palette.Colors.All(color => color.Locked))
        {
            return;
        }

        _generator.Generate(palette);
        context.Palettes.Update(palette);
        await context.SaveChangesAsync(cancellationToken);

        await using var stream = palette.GetImageStream(1000, 400);
        var @base = new InputMedia(stream, "palette");
        var media = new InputMediaPhoto(@base);

        await botClient.EditMessageMediaAsync(
            chatId, messageId, media,
            GetKeyboard(palette),
            cancellationToken);
    }
}
