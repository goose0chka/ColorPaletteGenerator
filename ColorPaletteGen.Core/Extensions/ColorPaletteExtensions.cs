using ColorPaletteGen.DAL.Model;

namespace ColorPaletteGen.Core.Extensions;

public static class ColorPaletteExtensions
{
    public static void InvertLock(this ColorPalette palette, int index)
    {
        if (index < 0 || index > palette.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
        }
        var color = palette.Colors[index];
        color.Locked = !color.Locked;
    }
}
