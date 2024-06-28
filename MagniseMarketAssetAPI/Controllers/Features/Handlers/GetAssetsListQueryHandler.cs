using AutoMapper;

public class GetAssetsListQueryHandler : IRequestHandler<GetAssetsListQuery, FintachartAPIResponseDTO>
{
    private readonly FintaChartsClientService _fintaChartsClientService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAssetsListQueryHandler(FintaChartsClientService fintaChartsClientService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _fintaChartsClientService = fintaChartsClientService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<FintachartAPIResponseDTO> Handle(GetAssetsListQuery request, CancellationToken cancellationToken)
    {
        var provider = request.Provider;
        var kind = request.Kind;
        var symbol = request.Symbol;
        var page = request.Page ?? 1; 
        var size = request.Size ?? 10;

        var apiResponse =  await _fintaChartsClientService.GetAssetsListAsync(provider, kind, symbol, page, size);

        foreach (var apiAsset in apiResponse.Data)
        {
            var asset = _mapper.Map<Asset>(apiAsset);

            if (string.IsNullOrEmpty(asset.BaseCurrency))
            {
                asset.BaseCurrency = "n/a"; 
            }

            var existingAsset = await _unitOfWork.Assets.GetByIdAsync(asset.Id);
            if (existingAsset == null)
            {
                await _unitOfWork.Assets.AddAsync(asset);
            }
            else
            {
                existingAsset.Kind = asset.Kind;
                existingAsset.Description = asset.Description;
                existingAsset.TickSize = asset.TickSize;
                existingAsset.Currency = asset.Currency;
                existingAsset.BaseCurrency = asset.BaseCurrency;

                existingAsset.Mappings.Clear();
                foreach (var mapping in asset.Mappings)
                {
                    existingAsset.Mappings.Add(mapping);
                }
            }
        }

        await _unitOfWork.CompleteAsync();

        return apiResponse;
    }
}