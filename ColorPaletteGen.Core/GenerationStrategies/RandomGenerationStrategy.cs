namespace ColorPaletteGen.Core.GenerationStrategies;

public class RandomGenerationStrategy : IGenerationStrategy
{
    private static readonly Random Rand = Random.Shared;
    private static readonly byte[] ByteBuffer = new byte[3];
    public List<Color> Generate(int count)
    {
        List <Color> colors = new(count);
        for (var i = 0; i < 5; i++)
        {
            Rand.NextBytes(ByteBuffer);
            colors.Add(Color.FromRGB(ByteBuffer));
        }

        return colors;
    }
}