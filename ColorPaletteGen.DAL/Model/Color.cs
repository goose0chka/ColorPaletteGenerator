using System.Text;

namespace ColorPaletteGen.DAL.Model;

public class Color
{
    public Color(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    public Color() 
        : this(0, 0, 0) { }

    public byte Red { get; set; }
    public byte Green { get; set; }
    public byte Blue { get; set; }
    public bool Locked { get; set; }

    public override string ToString()
        => $"#{Red:X2}{Green:X2}{Blue:X2}";

    public bool Equals(Color other)
        => Red == other.Red && Green == other.Green && Blue == other.Blue && Locked == other.Locked;
}
