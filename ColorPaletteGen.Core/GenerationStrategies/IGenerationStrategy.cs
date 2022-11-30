namespace ColorPaletteGen.Core.GenerationStrategies;

public interface IGenerationStrategy<out T> where T : Enum
{
    T Strategy { get; }
    public void Generate(ColorPalette palette);
}
