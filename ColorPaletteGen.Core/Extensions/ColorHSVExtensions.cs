namespace ColorPaletteGen.Core.Extensions;

public static class ColorHSVExtensions
{
    private const double Tolerance = 0.01;
    private static double Normalize(byte value)
        => value / 255.0; 
    public static int GetHue(this Color color)
    {
        var r = Normalize(color.Red);
        var g = Normalize(color.Green);
        var b = Normalize(color.Blue);
        
        // RGB to HSV
        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var diff = max - min;

        if (diff == 0)
        {
            return 0;
        }
        
        if (Math.Abs(max - r) < Tolerance)
        {
            return (int)Math.Truncate(60 * ((g - b) / diff % 6));
        }

        if (Math.Abs(max - g) < Tolerance)
        {
            return (int)Math.Truncate(60 * ((b - r) / diff + 2));
        }

        if (Math.Abs(max - b) < Tolerance)
        {
            return (int)Math.Truncate(60 * ((r - g) / diff + 4));
        }

        throw new Exception();
    }

    public static Color FromHSV(int hue, int saturation, int value)
    {
        var h = Math.Abs(hue % 360);
        var s = Math.Clamp(saturation, 0, 100) / 100.0;
        var v = Math.Clamp(value, 0, 100) / 100.0;
        
        // HSV to RGB 
        var c = v * s;
        var x = c * (1 - Math.Abs(h / 60.0 % 2 - 1));
        var m = v - c;
        double rN = 0, gN = 0, bN = 0;
        
        // TODO: Change to if's or smth
        switch (h / 60)
        {
            case 0:
                rN = c;
                gN = x;
                bN = 0;
                break;
            case 1:
                rN = x;
                gN = c;
                bN = 0;
                break;
            case 2:
                rN = 0;
                gN = c;
                bN = x;
                break;
            case 3:
                rN = 0;
                gN = x;
                bN = c;
                break;
            case 4:
                rN = x;
                gN = 0;
                bN = c;
                break;
            case 5:
                rN = c;
                gN = 0;
                bN = x;
                break;
        }

        var r = (byte)(Math.Round((rN + m) * 255) % 256);
        var g = (byte)(Math.Round((gN + m) * 255) % 256);
        var b = (byte)(Math.Round((bN + m) * 255) % 256);
        
        return Color.FromRGB(r, g, b);
    }
}