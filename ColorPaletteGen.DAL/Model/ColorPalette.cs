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
        if (count is < 2 or > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Color count must be in range from 2 to 10");
        }
        
        Colors = Enumerable
            .Range(0, count)
            .Select(_ => new Color())
            .ToArray();
    }
    
    private ColorPalette() { }
}
