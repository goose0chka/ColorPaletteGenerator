namespace ColorPaletteGen.Core.GenerationStrategies;

public class RandomGenerationStrategy : IGenerationStrategy
{
    private static readonly Random Rand = Random.Shared;
    private static readonly byte[] ByteBuffer = new byte[3];
    public void Generate(Color[] colors)
    {
        for (var i = 0; i < colors.Length; i++)
        {
            colors[i] = GetRandomColor();
        }
    }

    public static Color GetRandomColor()
    {
        Rand.NextBytes(ByteBuffer);
        return Color.FromRGB(ByteBuffer);
    }
}
