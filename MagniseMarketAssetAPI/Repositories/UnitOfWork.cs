public class UnitOfWork : IUnitOfWork
{
    private readonly MarketContext _context;
    private AssetRepository _assetRepository;
    private HistoricalPriceRepository _historicalPriceRepository;

    public UnitOfWork(MarketContext context)
    {
        _context = context;
    }

    public IAssetRepository Assets
    {
        get
        {
            return _assetRepository ??= new AssetRepository(_context);
        }
    }

    public IHistoricalPriceRepository HistoricalPrices
    {
        get
        {
            return _historicalPriceRepository ??= new HistoricalPriceRepository(_context);
        }
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}