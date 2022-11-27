namespace ColorPaletteGen.Core.GenerationStrategies;

public interface IGenerationStrategy
{
    public void Generate(Color[] colors);
}