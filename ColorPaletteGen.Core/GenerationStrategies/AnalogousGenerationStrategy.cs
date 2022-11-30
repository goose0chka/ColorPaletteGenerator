using ColorPaletteGen.Core.Color;
using ColorPaletteGen.Core.Extensions;

namespace ColorPaletteGen.Core.GenerationStrategies;

public class AnalogousGenerationStrategy : IGenerationStrategy<GenerationStrategy>
{
    private readonly BaseColor _baseColor;
    private readonly int _distance;

    public AnalogousGenerationStrategy(int distance = 15)
    {
        _baseColor = BaseColor.FromRGB(0, 0, 0);
        _distance = distance;
    }

    public GenerationStrategy Strategy => GenerationStrategy.Analogous;


    public void Generate(ColorPalette palette)
    {
        var colors = palette.Colors;
        if (colors.Length % 2 != 1)
        {
            throw new InvalidOperationException("Analogous palette should have uneven color count");
        }

        var sideCount = colors.Length / 2;
        var hue = _baseColor.GetHue();
        var sat = _baseColor.GetSaturation();
        var val = _baseColor.GetValue();
        for (var i = 0; i < colors.Length; i++)
        {
            if (colors[i].Locked)
            {
                continue;
            }

            var newHue = hue + _distance * (i - sideCount);
            var color = ColorHSVExtensions.FromHSV(newHue, sat, val);
            colors[i] = new PaletteColor(color);
        }
    }
}
