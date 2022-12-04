namespace ColorPaletteGen.DAL.Model;

public class ColorPalette
{
    public long Id { get; set; }
    public long ChatId { get; set; }
    public IList<Color> Colors { get; set; }
    public GenerationStrategy GenerationStrategy { get; set; }
    public int Count => Colors.Count;

    public ColorPalette(int count = 5)
    {
        Colors = Enumerable
            .Range(0, count)
            .Select(_ => new Color())
            .ToArray();
    }
}
