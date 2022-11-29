using Microsoft.VisualBasic;

namespace ColorPaletteGen.Core.Color;

public class BaseColor
{
    public byte Red { get; }
    public byte Green { get; }
    public byte Blue { get; }
    public string Hex => $"#{Red:X2}{Green:X2}{Blue:X2}";

    internal BaseColor(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    public static BaseColor FromRGB(byte red, byte green, byte blue)
        => new BaseColor(red, green, blue);

    public static BaseColor FromRGB(byte[] bytes)
    {
        if (bytes.Length != 3)
        {
            throw new InvalidOperationException();
        }
        
        return new BaseColor(bytes[0], bytes[1], bytes[2]);
    }

    public override string ToString()
        => Hex;
}
