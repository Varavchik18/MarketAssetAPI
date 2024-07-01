using System.ComponentModel.DataAnnotations.Schema;

public class HistoricalPrice
{
    public int Id { get; set; }
    public Guid AssetId { get; set; }
    public DateTimeOffset Time { get; set; }

    [Column(TypeName = "decimal(23, 15)")]
    public decimal Open { get; set; }

    [Column(TypeName = "decimal(23, 15)")]

    public decimal High { get; set; }

    [Column(TypeName = "decimal(23, 15)")]

    public decimal Low { get; set; }

    [Column(TypeName = "decimal(23, 15)")]
    public decimal Close { get; set; }

    public long Volume { get; set; }

    public Asset Asset { get; set; }
}
