﻿using ColorPaletteGen.Core.Color;

namespace ColorPaletteGen.Core.GenerationStrategies;

public class RandomGenerationStrategy : IGenerationStrategy<GenerationStrategy>
{
    private static readonly Random Rand = Random.Shared;
    private static readonly byte[] ByteBuffer = new byte[3];
    public GenerationStrategy Strategy => GenerationStrategy.Random;

    public void Generate(ColorPalette palette)
    {
        var colors = palette.Colors;
        for (var i = 0; i < colors.Length; i++)
        {
            if (colors[i].Locked)
            {
                continue;
            }

            colors[i] = new PaletteColor(GetRandomColor());
        }
    }

    private static BaseColor GetRandomColor()
    {
        Rand.NextBytes(ByteBuffer);
        return BaseColor.FromRGB(ByteBuffer);
    }
}
