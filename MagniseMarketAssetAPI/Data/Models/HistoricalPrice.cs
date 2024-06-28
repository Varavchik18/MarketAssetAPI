public class HistoricalPrice
{
    public int Id {  get; set; }
    public Guid AssetId { get; set; }
    public DateTimeOffset Time { get; set; }
    public decimal Open { get; set; } 
    public decimal High { get; set; } 
    public decimal Low { get; set; } 
    public decimal Close { get; set; } 
    public int Volume { get; set; }

    public Asset Asset { get; set; }
}
