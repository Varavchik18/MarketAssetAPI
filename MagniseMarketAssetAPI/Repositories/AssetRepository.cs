using Microsoft.EntityFrameworkCore;

public class AssetRepository : GenericRepository<Asset>, IAssetRepository
{
    public AssetRepository(MarketContext context) : base(context)
    {
    }

    public async Task<Asset> GetBySymbolAsync(string symbol)
    {
        return await _context.Assets
            .Include(a => a.Mappings)
            .FirstOrDefaultAsync(a => a.Symbol == symbol);
    }
    
    public async Task<Asset> GetByIdAsync(Guid assetId)
    {
        return await _context.Assets
            .Include(a => a.Mappings)
            .SingleOrDefaultAsync(a => a.Id == assetId);
    }
}