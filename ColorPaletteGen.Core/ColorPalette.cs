using System.Collections.ObjectModel;
using ColorPaletteGen.Core.GenerationStrategies;

namespace ColorPaletteGen.Core;

public class ColorPalette
{
    private List<Color> _colors;
    private IGenerationStrategy _strategy;
    public ReadOnlyCollection<Color> Colors => _colors.AsReadOnly();
    public int ColorCount => _colors.Capacity;
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

        _colors = new List<Color>(colorCount);
        _strategy = strategy;
        Generate();
    }

    public void SetStrategy(IGenerationStrategy strategy)
        => _strategy = strategy;

    public void Generate()
    {
        _colors = _strategy.Generate(ColorCount);
    }

    public Color this[int index]
        => _colors[index];
}