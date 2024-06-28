public interface IUnitOfWork : IDisposable
{
    IAssetRepository Assets { get; }
    IHistoricalPriceRepository HistoricalPrices { get; }
    Task<int> CompleteAsync();
}