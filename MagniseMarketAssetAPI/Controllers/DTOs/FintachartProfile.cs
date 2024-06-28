using AutoMapper;

public class FintachartProfile : Profile
{
    public FintachartProfile()
    {
        CreateMap<FintachartAssetDTO, Asset>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
            .ForMember(dest => dest.Mappings, opt => opt.MapFrom(src => src.Mappings.Select(m => new AssetMapping
            {
                MappingType = m.Key,
                Symbol = m.Value.Symbol,
                Exchange = m.Value.Exchange,
                DefaultOrderSize = m.Value.DefaultOrderSize
            }).ToList()));

        CreateMap<FintachartAssetMappingDTO, AssetMapping>();
    }
}
