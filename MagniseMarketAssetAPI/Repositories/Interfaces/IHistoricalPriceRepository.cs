public interface IHistoricalPriceRepository : IGenericRepository<HistoricalPrice>
{
    Task<IEnumerable<HistoricalPrice>> GetHistoricalPriceListByAssetId(Guid assetId);
    Task<HistoricalPrice> GetHistoricalPriceByAssetIdAndTime(Guid assetId, DateTimeOffset time);
}
