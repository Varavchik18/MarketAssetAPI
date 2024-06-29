
public class RealTimeDataStore
{
    private readonly Dictionary<string, RealTimePriceDataDTO> _data = new Dictionary<string, RealTimePriceDataDTO>();

    public void UpdateData(Guid instrumentId, PriceData last, PriceData ask, PriceData bid)
    {
        if (!_data.ContainsKey(instrumentId.ToString()))
        {
            _data[instrumentId.ToString()] = new RealTimePriceDataDTO();
        }

        var realTimeData = _data[instrumentId.ToString()];

        if (last != null)
        {
            realTimeData.Last = last;
        }

        if (ask != null)
        {
            realTimeData.Ask = ask;
        }

        if (bid != null)
        {
            realTimeData.Bid = bid;
        }
    }

    public RealTimePriceDataDTO GetData(string instrumentId)
    {
        return _data.ContainsKey(instrumentId) ? _data[instrumentId] : null;
    }
}