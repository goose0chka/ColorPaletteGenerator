namespace ColorPaletteGen.Core.Color;

public class PaletteColor : BaseColor
{
    internal PaletteColor(BaseColor color) 
        : base(color.Red, color.Green, color.Blue) { }

    public bool Locked { get; set; }
}
