using ColorPaletteGen.Core.Color;

namespace ColorPaletteGen.Core.GenerationStrategies;

public interface IGenerationStrategy
{
    public void Generate(PaletteColor[] colors);
}
