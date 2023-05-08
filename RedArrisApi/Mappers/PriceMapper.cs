using AutoMapper;
using RedArrisApi.Models;

namespace RedArrisApi.Mappers;

public class PriceMapper : Profile
{
    public PriceMapper()
    {
        ConfigureMappings();
    }

    private void ConfigureMappings()
    {
        CreateMap<IexPriceResponse, Return>()
            .ForMember(dest => dest.AsOfDate, map => map.MapFrom(src => DateTime.Parse(src.AsOfDate)))
            .ForMember(dest => dest.Ticker, map => map.MapFrom(src => src.Ticker))
            .ForMember(dest => dest.ClosePrice, map => map.MapFrom(src => src.Close));
    }
}