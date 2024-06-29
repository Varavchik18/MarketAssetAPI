/// <summary>
/// The RealTimeDataStore class manages the storage and retrieval of real-time price data for instruments.
/// </summary>
public class RealTimeDataStore
{
    private readonly Dictionary<string, RealTimePriceDataDTO> _data = new Dictionary<string, RealTimePriceDataDTO>();

    /// <summary>
    /// Updates the real-time data for a specific instrument.
    /// </summary>
    /// <param name="instrumentId">The ID of the instrument.</param>
    /// <param name="last">The last price data.</param>
    /// <param name="ask">The ask price data.</param>
    /// <param name="bid">The bid price data.</param>
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

    /// <summary>
    /// Retrieves the real-time price data for a specific instrument.
    /// </summary>
    /// <param name="instrumentId">The ID of the instrument.</param>
    /// <returns>A <see cref="RealTimePriceDataDTO"/> containing the real-time data for the instrument, or null if no data exists.</returns>
    public RealTimePriceDataDTO GetData(string instrumentId)
    {
        return _data.ContainsKey(instrumentId) ? _data[instrumentId] : null;
    }
}
