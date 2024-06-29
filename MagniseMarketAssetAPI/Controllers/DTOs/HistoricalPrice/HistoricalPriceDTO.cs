using System.Text.Json.Serialization;

public class HistoricalPriceDTO
{
    [JsonPropertyName("t")]
    public DateTime Time { get; set; }

    [JsonPropertyName("o")]
    public decimal Open { get; set; }

    [JsonPropertyName("h")]
    public decimal High { get; set; }

    [JsonPropertyName("l")]
    public decimal Low { get; set; }

    [JsonPropertyName("c")]
    public decimal Close { get; set; }

    [JsonPropertyName("v")]
    public long Volume { get; set; }
}