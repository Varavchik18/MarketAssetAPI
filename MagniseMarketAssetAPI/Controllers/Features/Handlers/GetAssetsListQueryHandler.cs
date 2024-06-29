using AutoMapper;

/// <summary>
/// Handles the GetAssetsListQuery by interacting with the FintaChartsClientService to retrieve a list of assets,
/// maps the retrieved assets to the Asset entity, and updates the database using the UnitOfWork pattern.
/// </summary>
public class GetAssetsListQueryHandler : IRequestHandler<GetAssetsListQuery, AssetsResponseDTO>
{
    private readonly FintaChartsClientService _fintaChartsClientService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAssetsListQueryHandler"/> class.
    /// </summary>
    /// <param name="fintaChartsClientService">The service to interact with Fintacharts API.</param>
    /// <param name="unitOfWork">The UnitOfWork instance for database operations.</param>
    /// <param name="mapper">The AutoMapper instance for mapping API responses to entity models.</param>
    public GetAssetsListQueryHandler(FintaChartsClientService fintaChartsClientService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _fintaChartsClientService = fintaChartsClientService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetAssetsListQuery request.
    /// </summary>
    /// <param name="request">The GetAssetsListQuery request containing parameters for filtering the assets list.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the assets response DTO.</returns>
    /// <exception cref="Exception">Thrown when the API response contains no data.</exception>
    public async Task<AssetsResponseDTO> Handle(GetAssetsListQuery request, CancellationToken cancellationToken)
    {
        var provider = request.Provider;
        var kind = request.Kind;
        var symbol = request.Symbol;
        var page = request.Page ?? 1;
        var size = request.Size ?? 10;

        var apiResponse = await _fintaChartsClientService.GetAssetsListAsync(provider, kind, symbol, page, size);

        if (apiResponse.Data.Count == 0) throw new Exception("Api response is null for values you provided");

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
