using Microsoft.EntityFrameworkCore;

namespace ColorPaletteGen.DAL.Context;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options) { }
}
