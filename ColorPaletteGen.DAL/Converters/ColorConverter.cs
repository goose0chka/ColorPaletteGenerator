using System.Text;
using ColorPaletteGen.DAL.Model;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ColorPaletteGen.DAL.Converters;

public class ColorConverter : ValueConverter<IList<Color>, string>
{
    public ColorConverter()
        : base(
            colors => GetColorString(colors), 
            s => RestoreColorsFromString(s)) { }

    private static string GetColorString(IEnumerable<Color> colors)
    {
        var builder = new StringBuilder();
        foreach (var color in colors)
        {
            var colorString = $"{color.Red:X2}{color.Green:X2}{color.Blue:X2}";
            builder.Append(colorString);
            if (color.Locked)
            {
                builder.Append('L');
            }
            builder.Append(';');
        }

        return builder.ToString();
    }

    private static IList<Color> RestoreColorsFromString(string s)
    {
        var strings = s.Split(';');
        var res = new Color[strings.Length];
        for (var i = 0; i < strings.Length; i++)
        {
            var str = strings[i];
            res[i] = new Color
            {
                Red = byte.Parse(str[..2]),
                Green = byte.Parse(str[2..4]),
                Blue = byte.Parse(str[4..6]),
                Locked = str[^1] == 'L'
            };
        }

        return res;
    }
}
