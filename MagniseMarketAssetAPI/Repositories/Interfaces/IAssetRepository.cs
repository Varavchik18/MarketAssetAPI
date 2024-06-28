public interface IAssetRepository : IGenericRepository<Asset>
{
    Task<Asset> GetBySymbolAsync(string symbol);
    Task<Asset> GetByIdAsync(Guid idAsset);
}