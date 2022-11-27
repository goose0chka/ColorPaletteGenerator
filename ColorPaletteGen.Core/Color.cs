namespace ColorPaletteGen.Core;

public readonly struct Color
{
    public byte Red { get; }
    public byte Green { get; }
    public byte Blue { get; }
    public string Hex => $"#{Red:X2}{Green:X2}{Blue:X2}";

    private Color(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    public static Color FromRGB(byte red, byte green, byte blue)
        => new Color(red, green, blue);

    public static Color FromRGB(byte[] bytes)
    {
        if (bytes.Length != 3)
        {
            throw new InvalidOperationException();
        }
        
        return new Color(bytes[0], bytes[1], bytes[2]);
    }

    public override string ToString()
        => Hex;
}
