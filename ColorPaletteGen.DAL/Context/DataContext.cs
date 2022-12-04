using ColorPaletteGen.DAL.Comparers;
using ColorPaletteGen.DAL.Model;
using Microsoft.EntityFrameworkCore;
using ColorPaletteGen.DAL.Converters;

namespace ColorPaletteGen.DAL.Context;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options) { }

    public DbSet<ColorPalette> Palettes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var paletteEntity = modelBuilder
            .Entity<ColorPalette>();
        paletteEntity
            .HasKey(palette => new { palette.ChatId, palette.Id });
        paletteEntity
            .Property(palette => palette.Colors)
            .HasConversion<ColorConverter, ColorComparer>();

    }
}
