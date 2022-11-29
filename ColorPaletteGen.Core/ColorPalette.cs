using System.Collections.ObjectModel;
using ColorPaletteGen.Core.Color;
using ColorPaletteGen.Core.GenerationStrategies;

namespace ColorPaletteGen.Core;

public class ColorPalette
{
    private readonly PaletteColor[] _colors;
    private IGenerationStrategy _strategy;

    public ReadOnlyCollection<BaseColor> Colors => _colors
        .Cast<BaseColor>()
        .ToList()
        .AsReadOnly();

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

        _colors = new PaletteColor[colorCount];
        _strategy = strategy;
        Generate();
    }

    public void SetStrategy(IGenerationStrategy strategy)
        => _strategy = strategy;

    public void Generate()
    {
        _strategy.Generate(_colors);
    }

    public BaseColor this[int index]
        => _colors[index];
}
