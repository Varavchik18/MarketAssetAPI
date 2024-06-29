using Microsoft.EntityFrameworkCore;

/// <summary>
/// The HistoricalPriceRepository class provides methods to interact with HistoricalPrice entities in the database.
/// Inherits from GenericRepository and implements IHistoricalPriceRepository.
/// </summary>
public class HistoricalPriceRepository : GenericRepository<HistoricalPrice>, IHistoricalPriceRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HistoricalPriceRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public HistoricalPriceRepository(MarketContext context) : base(context)
    {
    }

    /// <summary>
    /// Asynchronously gets a list of historical prices by asset identifier.
    /// </summary>
    /// <param name="assetId">The identifier of the asset.</param>
    /// <returns>A task that represents the asynchronous operation, containing an enumerable of historical prices.</returns>
    public async Task<IEnumerable<HistoricalPrice>> GetHistoricalPriceListByAssetId(Guid assetId)
    {
        return await _context.HistoricalPrices.Where(h => h.AssetId == assetId).ToListAsync();
    }
}
