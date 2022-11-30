using ColorPaletteGen.Core.Color;

namespace ColorPaletteGen.Core.Extensions;

public static class ColorHSVExtensions
{
    private const double Tolerance = 0.01;

    private static double Normalize(byte value)
        => value / 255.0;

    private readonly struct RGBToHSVData
    {
        public readonly double R;
        public readonly double G;
        public readonly double B;
        public double Max => Math.Max(R, Math.Max(G, B));
        public double Min => Math.Min(R, Math.Min(G, B));
        public double Diff => Max - Min;

        public RGBToHSVData(BaseColor color)
        {
            R = Normalize(color.Red);
            G = Normalize(color.Green);
            B = Normalize(color.Blue);
        }
    }

    public static int GetHue(this BaseColor color)
    {
        var data = new RGBToHSVData(color);

        if (data.Diff == 0)
        {
            return 0;
        }

        if (Math.Abs(data.Max - data.R) < Tolerance)
        {
            return (int)Math.Truncate(60 * ((data.G - data.B) / data.Diff % 6));
        }

        if (Math.Abs(data.Max - data.G) < Tolerance)
        {
            return (int)Math.Truncate(60 * ((data.B - data.R) / data.Diff + 2));
        }

        if (Math.Abs(data.Max - data.B) < Tolerance)
        {
            return (int)Math.Truncate(60 * ((data.R - data.G) / data.Diff + 4));
        }

        throw new Exception();
    }

    public static int GetSaturation(this BaseColor color)
    {
        var data = new RGBToHSVData(color);
        return data.Max == 0 ? 0 : Convert.ToInt32(data.Max / data.Diff * 100);
    }

    public static int GetValue(this BaseColor color)
        => Convert.ToInt32(new RGBToHSVData(color).Max * 100);

    public static BaseColor FromHSV(int hue, int saturation, int value)
    {
        var h = hue >= 0 ? hue % 360 : 360 - Math.Abs(hue % 360);
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

        var r = Convert.ToByte(Math.Round((rN + m) * 255) % 256);
        var g = Convert.ToByte(Math.Round((gN + m) * 255) % 256);
        var b = Convert.ToByte(Math.Round((bN + m) * 255) % 256);

        return BaseColor.FromRGB(r, g, b);
    }
}
