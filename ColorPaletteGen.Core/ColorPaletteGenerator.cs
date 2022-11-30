using ColorPaletteGen.Core.GenerationStrategies;

namespace ColorPaletteGen.Core;

public class ColorPaletteGenerator
{
    private readonly IEnumerable<IGenerationStrategy<GenerationStrategy>> _strategies;

    public ColorPaletteGenerator(IEnumerable<IGenerationStrategy<GenerationStrategy>> strategies)
    {
        _strategies = strategies;
    }

    public void Generate(ColorPalette palette)
    {
        var strategy = _strategies.First(x => x.Strategy == palette.Strategy);
        strategy.Generate(palette);
    }
}
