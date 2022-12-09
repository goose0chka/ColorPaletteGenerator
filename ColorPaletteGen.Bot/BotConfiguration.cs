namespace ColorPaletteGen.Bot;

public class BotConfiguration
{
    public const string ConfigurationSection = "BotConfig";
    public string Token { get; set; }
    public string Host { get; set; }
    public string Endpoint { get; set; }
}
