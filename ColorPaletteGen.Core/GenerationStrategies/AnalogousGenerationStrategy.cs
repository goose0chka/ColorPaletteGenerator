using ColorPaletteGen.Core.Extensions;

namespace ColorPaletteGen.Core.GenerationStrategies;

public class AnalogousGenerationStrategy : IGenerationStrategy
{
    private readonly int _distance;
    private readonly Color _baseColor;

    public AnalogousGenerationStrategy(Color baseColor, int distance = 15)
    {
        _baseColor = baseColor;
        _distance = distance;
    }

    public void Generate(Color[] colors)
    {
        // throw new NotImplementedException();
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
            var newHue = hue + _distance * (i - sideCount);
            var color = ColorHSVExtensions.FromHSV(newHue, sat, val);
            colors[i] = color;
        }
    }
}