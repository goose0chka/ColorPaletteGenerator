using ColorPaletteGen.Core.Extensions;
using ColorPaletteGen.DAL.Model;

namespace ColorPaletteGen.Core.GenerationStrategies;

public class RandomGenerationStrategy : IGenerationStrategy<GenerationStrategy>
{
    private static readonly Random Rand = Random.Shared;
    private static readonly byte[] ByteBuffer = new byte[3];
    public GenerationStrategy Strategy => GenerationStrategy.Random;

    public void Generate(ColorPalette palette)
    {
        var colors = palette.Colors;
        for (var i = 0; i < colors.Count; i++)
        {
            if (colors[i].Locked)
            {
                continue;
            }
    
            colors[i] = GetRandomColor();
        }
    }

    private static Color GetRandomColor()
    {
        Rand.NextBytes(ByteBuffer);
        return ColorExtensions.FromByteArray(ByteBuffer);
    }
}
