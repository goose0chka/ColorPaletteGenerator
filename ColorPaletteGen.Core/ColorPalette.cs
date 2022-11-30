using System.Collections.ObjectModel;
using ColorPaletteGen.Core.Color;
using ColorPaletteGen.Core.GenerationStrategies;

namespace ColorPaletteGen.Core;

public class ColorPalette
{
    public readonly PaletteColor[] Colors;
    public readonly GenerationStrategy Strategy;

    public ColorPalette(int colorCount = 5, GenerationStrategy strategy = GenerationStrategy.Random)
    {
        if (colorCount is > 10 or < 2)
        {
            throw new InvalidOperationException();
        }

        Strategy = strategy;
        Colors = Enumerable.Range(0, colorCount)
            .Select(_ => new PaletteColor())
            .ToArray();
    }

    public int ColorCount => Colors.Length;

    public BaseColor this[int index]
        => Colors[index];

    public void LockColor(int index, bool locked = true)
    {
        if (index > Colors.Length)
        {
            throw new ArgumentException("Color index is out of range");
        }

        Colors[index].Locked = locked;
    }

    public void InvertLock(int index)
    {
        var newLock = !Colors[index].Locked;
        LockColor(index, newLock);
    }
}
