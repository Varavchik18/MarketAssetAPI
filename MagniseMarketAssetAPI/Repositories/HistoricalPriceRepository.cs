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

    public async Task<HistoricalPrice> GetHistoricalPriceByAssetIdAndTime(Guid assetId, DateTimeOffset time)
    {
        return await _context.HistoricalPrices
            .FirstOrDefaultAsync(hp => hp.AssetId == assetId && hp.Time == time);
    }

}