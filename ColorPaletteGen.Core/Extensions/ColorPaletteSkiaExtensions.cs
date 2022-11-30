using ColorPaletteGen.Core.Color;
using SkiaSharp;

namespace ColorPaletteGen.Core.Extensions;

public static class ColorPaletteSkiaExtensions
{
    public static async Task GenerateImageFileAsync(this ColorPalette palette, int width, int height, string path)
    {
        using var data = GetData(palette, width, height);
        path = GetPath(path);

        await using var stream = File.OpenWrite(path);
        data.SaveTo(stream);
    }

    private static string GetPath(string path)
    {
        var directory = Path.GetDirectoryName(path)
            ?? throw new ArgumentException("Directory cannot be null");
        var file = Path.GetFileName(path);
        
        if (Path.GetExtension(file) != "png")
        {
            if (Path.HasExtension(file))
            {
                throw new ArgumentException("File must have png extension");
            }

            directory = Path.Join(directory, file);
            file = Path.ChangeExtension(Path.GetRandomFileName(), "png");
        }

        Directory.CreateDirectory(directory);
        return Path.Combine(directory, file);
    }

    public static Stream GetImageStream(this ColorPalette palette, int width, int height) 
        => GetData(palette, width, height).AsStream();

    private static SKData GetData(ColorPalette palette, int width, int height)
    {
        using SKBitmap bitmap = new(width, height);
        using SKCanvas canvas = new(bitmap);

        var widthPerColor = width / palette.ColorCount;
        for (var i = 0; i < palette.ColorCount; i++)
        {
            var x = widthPerColor * i;
            SKPaint pen = new()
            {
                Color = palette[i].ToSkiaColor(),
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRect(x, 0, widthPerColor, height, pen);
        }

        return bitmap.Encode(SKEncodedImageFormat.Png, 20);
    }

    private static SKColor ToSkiaColor(this BaseColor c)
        => new (c.Red, c.Green, c.Blue);
}
