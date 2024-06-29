using AutoMapper;

public class GetPricesDateRangeQueryHandler : IRequestHandler<GetPricesDateRangeQuery, PricesResponseDTO>
{
    private readonly FintaChartsClientService _fintaChartsClientService;
    private readonly FintaChartsClientService_WS _fintaChartsClientService_WS;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPricesDateRangeQueryHandler(FintaChartsClientService fintaChartsClientService, IUnitOfWork unitOfWork, IMapper mapper, FintaChartsClientService_WS fintaChartsClientService_WS)
    {
        _fintaChartsClientService = fintaChartsClientService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fintaChartsClientService_WS = fintaChartsClientService_WS;
    }

    public async Task<PricesResponseDTO> Handle(GetPricesDateRangeQuery request, CancellationToken cancellationToken)
    {
        var historicalData = await _fintaChartsClientService.GetHistoricalPricesDateRangeAsync(request.InstrumentId, request.Provider, request.Interval, request.Periodicity, request.StartDate, request.EndDate);
        RealTimePriceDataDTO realTimeData = await GetRealTimePriceData(request.InstrumentId, request.Provider);

        return new PricesResponseDTO
        {
            AssetId = request.InstrumentId,
            HistoricalData = historicalData,
            RealTimeData = realTimeData
        };
    }

    private async Task<RealTimePriceDataDTO> GetRealTimePriceData(string instrumentId, string provider)
    {
        await _fintaChartsClientService_WS.SubscribeAsync(instrumentId, provider);

        RealTimePriceDataDTO realTimeData;
        var startTime = DateTime.UtcNow;
        int timeoutSeconds = 10;
        while ((realTimeData = _fintaChartsClientService_WS.GetRealTimeData(instrumentId)) == null ||
               realTimeData.Last == null || realTimeData.Ask == null || realTimeData.Bid == null)
        {
            if ((DateTime.UtcNow - startTime).TotalSeconds > timeoutSeconds)
            {
                return new RealTimePriceDataDTO
                {
                    Last = null,
                    Ask = null,
                    Bid = null
                };
            }
            await Task.Delay(100);
        }

        return realTimeData;
    }
}