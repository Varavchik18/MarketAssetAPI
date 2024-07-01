using AutoMapper;

public class GetPricesDateRangeQueryHandler : IRequestHandler<GetPricesDateRangeQuery, PricesResponseDTO>
{
    private readonly FintaChartsClientService _fintaChartsClientService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPricesDateRangeQueryHandler(FintaChartsClientService fintaChartsClientService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _fintaChartsClientService = fintaChartsClientService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PricesResponseDTO> Handle(GetPricesDateRangeQuery request, CancellationToken cancellationToken)
    {
        var historicalData = await _fintaChartsClientService.GetHistoricalPricesDateRangeAsync(request.InstrumentId, request.Provider, request.Interval, request.Periodicity, request.StartDate, request.EndDate);

        foreach (var item in historicalData)
        {
            var existingRecord = await _unitOfWork.HistoricalPrices.GetHistoricalPriceByAssetIdAndTime(new Guid(request.InstrumentId), item.Time);
            if (existingRecord != null)
            {
                existingRecord.Open = item.Open;
                existingRecord.Close = item.Close;
                existingRecord.High = item.High;
                existingRecord.Low = item.Low;
                existingRecord.Volume = item.Volume;

            }
            else
            {
                await _unitOfWork.HistoricalPrices.AddAsync(new HistoricalPrice
                {
                    AssetId = new Guid(request.InstrumentId),
                    Open = item.Open,
                    Close = item.Close,
                    High = item.High,
                    Low = item.Low,
                    Time = item.Time,
                    Volume = item.Volume
                });
            }
        }

        await _unitOfWork.CompleteAsync();

        return new PricesResponseDTO
        {
            AssetId = request.InstrumentId,
            HistoricalData = historicalData
        };
    }

}