namespace ColorPaletteGen.Core.GenerationStrategies;

public interface IGenerationStrategy
{
    public abstract List<Color> Generate(int count);
}