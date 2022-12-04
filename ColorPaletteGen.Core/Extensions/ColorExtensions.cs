using ColorPaletteGen.DAL.Model;

namespace ColorPaletteGen.Core.Extensions;

public static class ColorExtensions
{
    public static Color FromByteArray(byte[] bytes)
    {
        if (bytes.Length != 3)
        {
            throw new InvalidOperationException();
        }

        return new Color
        {
            Red = bytes[0], 
            Green = bytes[1], 
            Blue = bytes[2]
        };
    }
}
