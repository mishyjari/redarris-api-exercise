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

        // Take the previousPrice and currenPrice in as a Tuple to calculate daily return
        CreateMap<Tuple<IexPriceDto, IexPriceDto>, Return>()
            .ForMember(dest => dest.Value, map =>
                map.MapFrom(src =>
                    CalculateReturn(src.Item2.ClosePrice, src.Item1.ClosePrice)))
            .ForMember(dest => dest.ClosePrice, map =>
                map.MapFrom(src => src.Item2.ClosePrice))
            .ForMember(dest => dest.AsOfDate, map =>
                map.MapFrom(src => DateTime.Parse(src.Item2.AsOfDate)));
    }

    // Note that we are using the return as a number-percentage to four decimal places, i.e., 2.15 means 2.15%
    // Remove the * 100 if we just want to use the percentage as its own decimal, i.e., 0.0215
    private double CalculateReturn(double previousPrice, double currentPrice) =>
        Math.Round((currentPrice / previousPrice - 1) * 100, 4);
}