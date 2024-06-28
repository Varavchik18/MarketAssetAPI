using Microsoft.EntityFrameworkCore;

public class HistoricalPriceRepository : GenericRepository<HistoricalPrice>, IHistoricalPriceRepository
{
    public HistoricalPriceRepository(MarketContext context) : base(context)
    {
    }

    public async Task<IEnumerable<HistoricalPrice>> GetHistoricalPriceListByAssetId(Guid assetId)
    {
        return await _context.HistoricalPrices.Where(h => h.AssetId == assetId).ToListAsync();
    }
}