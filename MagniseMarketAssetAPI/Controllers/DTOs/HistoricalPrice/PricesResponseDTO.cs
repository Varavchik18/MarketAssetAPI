public class PricesResponseDTO
{
    public string AssetId { get; set; }
    public List<HistoricalPriceDTO> HistoricalData { get; set; }
}