using Microsoft.EntityFrameworkCore;

/// <summary>
/// The AssetRepository class provides methods to interact with Asset entities in the database.
/// Inherits from GenericRepository and implements IAssetRepository.
/// </summary>
public class AssetRepository : GenericRepository<Asset>, IAssetRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssetRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AssetRepository(MarketContext context) : base(context)
    {
    }

    /// <summary>
    /// Asynchronously gets an asset by its symbol.
    /// </summary>
    /// <param name="symbol">The symbol of the asset.</param>
    /// <returns>A task that represents the asynchronous operation, containing the asset if found.</returns>
    public async Task<Asset> GetBySymbolAsync(string symbol)
    {
        return await _context.Assets
            .Include(a => a.Mappings)
            .FirstOrDefaultAsync(a => a.Symbol == symbol);
    }

    /// <summary>
    /// Asynchronously gets an asset by its identifier.
    /// </summary>
    /// <param name="assetId">The identifier of the asset.</param>
    /// <returns>A task that represents the asynchronous operation, containing the asset if found.</returns>
    public async Task<Asset> GetByIdAsync(Guid assetId)
    {
        return await _context.Assets
            .Include(a => a.Mappings)
            .SingleOrDefaultAsync(a => a.Id == assetId);
    }
}
