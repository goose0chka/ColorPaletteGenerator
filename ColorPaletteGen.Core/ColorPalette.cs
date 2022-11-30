using System.Collections.ObjectModel;
using ColorPaletteGen.Core.Color;
using ColorPaletteGen.Core.GenerationStrategies;

namespace ColorPaletteGen.Core;

public class ColorPalette
{
    private readonly PaletteColor[] _colors;
    private IGenerationStrategy _strategy;

    public ReadOnlyCollection<PaletteColor> Colors => Array.AsReadOnly(_colors);
    public int ColorCount => _colors.Length;

    public ColorPalette(int colorCount = 5)
        : this(new RandomGenerationStrategy(), colorCount)
    {
    }

    public ColorPalette(IGenerationStrategy strategy, int colorCount = 5)
    {
        if (colorCount is > 10 or < 2)
        {
            throw new InvalidOperationException();
        }

        _colors = Enumerable.Range(0, colorCount)
            .Select(_ => new PaletteColor())
            .ToArray();
        _strategy = strategy;
    }

    public void SetStrategy(IGenerationStrategy strategy)
        => _strategy = strategy;

    public void LockColor(int index, bool locked = true)
    {
        if (index > _colors.Length)
        {
            throw new ArgumentException("Color index is out of range");
        }

        _colors[index].Locked = locked;
    }

    public void InvertLock(int index)
    {
        var newLock = !_colors[index].Locked;
        LockColor(index, newLock);
    }

    public void Generate()
    {
        _strategy.Generate(_colors);
    }

    public BaseColor this[int index]
        => _colors[index];
}
