using AutoMapper;
using RedArrisApi.Models;

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
        // We need at least two values to calculate return, throw exception (caught in controller) if we don't meet this condition
        if (prices.Length < 2)
            throw new ArgumentOutOfRangeException(nameof(prices),
                "At least two price entities are required to calculate returns, however fewer than two results were returned from Price Service.");

        var returns = new Return[prices.Length - 1];

        // Parallel is great for larger data sets, but could take a slight performance hit with smaller sets, would be interesting to test run times versus a normal loop
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