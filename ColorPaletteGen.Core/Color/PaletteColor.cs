namespace ColorPaletteGen.Core.Color;

public class PaletteColor : BaseColor
{
    public PaletteColor()
        : base(0, 0, 0) { }

    internal PaletteColor(BaseColor color)
        : base(color.Red, color.Green, color.Blue) { }

    public bool Locked { get; set; }
}
