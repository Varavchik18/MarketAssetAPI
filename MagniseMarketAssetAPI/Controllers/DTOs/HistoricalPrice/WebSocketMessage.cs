using System.Text.Json.Serialization;

public class WebSocketMessage
{
    public string Type { get; set; }
    public Guid InstrumentId { get; set; }
    public string Provider { get; set; }
    [JsonPropertyName("last")]
    public PriceData Last { get; set; }
    [JsonPropertyName("ask")]
    public PriceData Ask { get; set; }
    [JsonPropertyName("bid")]
    public PriceData Bid { get; set; }
}