public class PricesResponseDTO
{
    public string AssetId { get; set; }
    public RealTimePriceDataDTO RealTimeData { get; set; }
    public List<HistoricalPriceDTO> HistoricalData { get; set; }
}