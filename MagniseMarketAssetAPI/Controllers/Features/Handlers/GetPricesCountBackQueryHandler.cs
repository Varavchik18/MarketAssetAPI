using AutoMapper;

/// <summary>
/// Handles the GetPricesCountBackQuery by retrieving historical and real-time price data for a given instrument.
/// </summary>
public class GetPricesCountBackQueryHandler : IRequestHandler<GetPricesCountBackQuery, PricesResponseDTO>
{
    private readonly FintaChartsClientService _fintaChartsClientService;
    private readonly FintaChartsClientService_WS _fintaChartsClientService_WS;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPricesCountBackQueryHandler"/> class.
    /// </summary>
    /// <param name="fintaChartsClientService">The service to interact with Fintacharts API for historical data.</param>
    /// <param name="unitOfWork">The UnitOfWork instance for database operations.</param>
    /// <param name="mapper">The AutoMapper instance for mapping API responses to entity models.</param>
    /// <param name="fintaChartsClientService_WS">The service to interact with Fintacharts API for real-time data.</param>
    public GetPricesCountBackQueryHandler(FintaChartsClientService fintaChartsClientService, IUnitOfWork unitOfWork, IMapper mapper, FintaChartsClientService_WS fintaChartsClientService_WS)
    {
        _fintaChartsClientService = fintaChartsClientService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fintaChartsClientService_WS = fintaChartsClientService_WS;
    }

    /// <summary>
    /// Handles the GetPricesCountBackQuery request.
    /// </summary>
    /// <param name="request">The GetPricesCountBackQuery request containing parameters for retrieving price data.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the prices response DTO.</returns>
    public async Task<PricesResponseDTO> Handle(GetPricesCountBackQuery request, CancellationToken cancellationToken)
    {
        var historicalData = await _fintaChartsClientService.GetHistoricalPricesCountBackAsync(
            request.InstrumentId, request.Provider, request.Interval, request.Periodicity, request.BarsCount);

        var realTimeData = await GetRealTimePriceData(request.InstrumentId, request.Provider);

        return new PricesResponseDTO
        {
            AssetId = request.InstrumentId,
            HistoricalData = historicalData,
            RealTimeData = realTimeData
        };
    }

    /// <summary>
    /// Retrieves real-time price data for a given instrument.
    /// </summary>
    /// <param name="instrumentId">The ID of the instrument.</param>
    /// <param name="provider">The provider of the instrument data.</param>
    /// <returns>A task that represents the asynchronous operation, containing the real-time price data DTO.</returns>
    private async Task<RealTimePriceDataDTO> GetRealTimePriceData(string instrumentId, string provider)
    {
        await _fintaChartsClientService_WS.SubscribeAsync(instrumentId, provider);

        RealTimePriceDataDTO realTimeData;
        while ((realTimeData = _fintaChartsClientService_WS.GetRealTimeData(instrumentId)) == null ||
               realTimeData.Last == null || realTimeData.Ask == null || realTimeData.Bid == null)
        {
            await Task.Delay(100);
        }

        return realTimeData;
    }
}
