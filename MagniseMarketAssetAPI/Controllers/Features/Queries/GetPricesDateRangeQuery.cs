public class GetPricesDateRangeQuery : IRequest<PricesResponseDTO>
{
    public string InstrumentId { get; set; }
    public string Provider { get; set; }
    public int Interval { get; set; }
    public string Periodicity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
