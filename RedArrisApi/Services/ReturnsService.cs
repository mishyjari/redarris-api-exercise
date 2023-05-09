using AutoMapper;

namespace RedArrisApi;

public class ReturnsService : IReturnsService
{
    private readonly IMapper _mapper;

    public ReturnsService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public IEnumerable<Return> CalculateReturns(IexPriceDto[] prices)
    {
        // Since we know the length of our data set, we can make use of the performance benefit of an array versus a list
        var returns = new Return[prices.Length - 1];

        // Parallel is great for larger data sets, but could take a slight performance hit with smaller sets
        // Would be interesting to test run times with different data set sizes versus a normal linear loop.
        Parallel.For(1, prices.Length, i =>
        {
            var previousPrice = prices[i - 1];
            var currentPrice = prices[i];

            // Calculation is handled within the AutoMapper Profile
            var dailyReturn = _mapper.Map<Return>(new Tuple<IexPriceDto, IexPriceDto>(previousPrice, currentPrice));

            // Insert result into the matching index in the array to preserve order
            returns[i - 1] = dailyReturn;
        });

        return returns;
    }
}