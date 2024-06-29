public class GetAssetsListQuery : IRequest<AssetsResponseDTO>
{
    public string? Provider { get; set; } = null;
    public string? Kind { get; set; } = null;
    public string? Symbol { get; set; } = null;
    public int? Page { get; set; }
    public int? Size { get; set; }
}
