public class FintachartAPIResponseDTO
{
    public Paging Paging { get; set; }
    public List<FintachartAssetDTO> Data { get; set; }
}

public class Paging
{
    public int Page { get; set; }
    public int Pages { get; set; }
    public int Items { get; set; }
}