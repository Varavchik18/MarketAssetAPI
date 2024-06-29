using System;
using System.Threading.Tasks;

/// <summary>
/// The UnitOfWork class coordinates the repositories and handles transactions.
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly MarketContext _context;
    private AssetRepository _assetRepository;
    private HistoricalPriceRepository _historicalPriceRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UnitOfWork(MarketContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the asset repository.
    /// </summary>
    public IAssetRepository Assets
    {
        get
        {
            return _assetRepository ??= new AssetRepository(_context);
        }
    }

    /// <summary>
    /// Gets the historical price repository.
    /// </summary>
    public IHistoricalPriceRepository HistoricalPrices
    {
        get
        {
            return _historicalPriceRepository ??= new HistoricalPriceRepository(_context);
        }
    }

    /// <summary>
    /// Saves all changes made in the context to the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, containing the number of state entries written to the database.</returns>
    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Disposes the context.
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
    }
}
