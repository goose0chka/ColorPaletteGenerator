using ColorPaletteGen.Core.Color;

namespace ColorPaletteGen.Core.GenerationStrategies;

public class RandomGenerationStrategy : IGenerationStrategy
{
    private static readonly Random Rand = Random.Shared;
    private static readonly byte[] ByteBuffer = new byte[3];
    public void Generate(PaletteColor[] colors)
    {
        for (var i = 0; i < colors.Length; i++)
        {
            colors[i] =  new PaletteColor(GetRandomColor());
        }
    }

    private static BaseColor GetRandomColor()
    {
        Rand.NextBytes(ByteBuffer);
        return BaseColor.FromRGB(ByteBuffer);
    }
}
