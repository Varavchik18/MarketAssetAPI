using Microsoft.EntityFrameworkCore;

public class MarketContext : DbContext
{
    public MarketContext(DbContextOptions<MarketContext> options) : base(options) { }

    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetMapping> AssetMappings { get; set; }
    public DbSet<HistoricalPrice> HistoricalPrices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Asset>()
            .HasMany(a => a.Mappings)
            .WithOne(m => m.Asset)
            .HasForeignKey(m => m.AssetId);

        modelBuilder.Entity<Asset>()
            .HasMany(a => a.HistoricalPrices)
            .WithOne(h => h.Asset)
            .HasForeignKey(h => h.AssetId);

        modelBuilder.Entity<AssetMapping>()
            .HasKey(am => am.Id);

        modelBuilder.Entity<HistoricalPrice>()
            .HasKey(hp => hp.Id);

        modelBuilder.Entity<HistoricalPrice>()
            .Property(hp => hp.Open)
            .HasColumnType("decimal(23, 15)");

        modelBuilder.Entity<HistoricalPrice>()
            .Property(hp => hp.High)
            .HasColumnType("decimal(23, 15)");

        modelBuilder.Entity<HistoricalPrice>()
            .Property(hp => hp.Low)
            .HasColumnType("decimal(23, 15)");

        modelBuilder.Entity<HistoricalPrice>()
            .Property(hp => hp.Close)
            .HasColumnType("decimal(23, 15)");

        base.OnModelCreating(modelBuilder);
    }

}
