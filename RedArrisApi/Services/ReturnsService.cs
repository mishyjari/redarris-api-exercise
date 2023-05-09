using AutoMapper;

namespace RedArrisApi;

public class ReturnsService : IReturnsService
{
    private readonly IMapper _mapper;

    public ReturnsService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public IEnumerable<Return> CalculateReturns(IexPriceResponse[] prices)
    {
        // IEX provides a query param to sort results, so this is not needed now,
        // However, since our data depends on sequential market day calculations, it may be worth the performance it to confirm we are sorted
        // prices = prices.OrderBy(p => p.AsOfDate).ToArray();
        
        // Since we know the length of our data set, we can make use of the performance benefit of an array versus a list
        var returns = new Return[prices.Length - 1];

        // Parallel is great for larger data sets, but could take a slight performance hit with smaller sets
        // Would be interesting to test run times with different data set sizes versus a normal linear loop.
        Parallel.For(1, prices.Length, i =>
        {
            var currentPrice = prices[i];
            var previousPrice = prices[i - 1];

            // Note that we are using the return as a number-percentage to four decimal places, i.e., 2.15 means 2.15%
            // Remove the * 100 if we just want to use the percentage as its own decimal, i.e., 0.0215
            var dailyReturnValue = (currentPrice.Close / previousPrice.Close - 1) * 100;
            var dailyReturn = _mapper.Map<Return>(currentPrice);
            dailyReturn.Value = Math.Round(dailyReturnValue, 4);

            // Insert result into the matching index in the array to preserve order
            returns[i - 1] = dailyReturn;
        });

        return returns;
    }
}