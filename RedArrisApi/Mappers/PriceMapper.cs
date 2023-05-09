using AutoMapper;

namespace RedArrisApi;

internal class PriceMapper : Profile
{
    public PriceMapper()
    {
        ConfigureMappings();
    }

    private void ConfigureMappings()
    {
        CreateMap<IexPriceResponse, Return>()
            .ForMember(dest => dest.AsOfDate, map => map.MapFrom(src => DateTime.Parse(src.AsOfDate)))
            .ForMember(dest => dest.ClosePrice, map => map.MapFrom(src => src.Close));
    }
}