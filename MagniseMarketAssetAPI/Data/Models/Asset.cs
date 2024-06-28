public class Asset
{
    public Guid Id { get; set; } 
    public string Symbol { get; set; } 
    public string Kind { get; set; } 
    public string Description { get; set; }
    public decimal TickSize { get; set; }
    public string Currency { get; set; } 
    public string BaseCurrency { get; set; }
    public ICollection<AssetMapping> Mappings { get; set; }
    public ICollection<HistoricalPrice> HistoricalPrices { get; set; }
}

public class AssetMapping
{
    public int Id { get; set; }
    public Guid AssetId { get; set; }
    public string MappingType { get; set; }
    public string Symbol { get; set; }
    public string Exchange { get; set; }
    public int DefaultOrderSize { get; set; }
    public Asset Asset { get; set; }
}