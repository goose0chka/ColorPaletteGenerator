using SkiaSharp;

namespace ColorPaletteGen.Core;

public static class ColorPaletteSkiaExtensions
{
    public static async Task GenerateImageFileAsync(this ColorPalette palette, int width, int height, string path)
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

        using var data = bitmap.Encode(SKEncodedImageFormat.Png, 80);

        if (Path.GetExtension(path) != "png")
        {
            path = Path.ChangeExtension(path, "png");
        }

        await using var stream = File.OpenWrite(path);
        data.SaveTo(stream);
    }
    
    private static SKColor ToSkiaColor(this Color c)
        => new SKColor(c.Red, c.Green, c.Blue);
}