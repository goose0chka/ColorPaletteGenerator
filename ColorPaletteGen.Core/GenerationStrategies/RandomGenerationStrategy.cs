namespace ColorPaletteGen.Core.GenerationStrategies;

public class RandomGenerationStrategy : IGenerationStrategy
{
    private static readonly Random Rand = new();
    public List<Color> Generate(int count)
    {
        List <Color> colors = new(count);
        var bytes = new byte[3];
        for (var i = 0; i < 5; i++)
        {
            Rand.NextBytes(bytes);
            colors.Add(Color.FromRGB(bytes));
        }

        return colors;
    }
}