using RedArrisApi.Models;

namespace RedArrisApi;

public interface IReturnsService
{
    IEnumerable<Return> CalculateReturns(IexPriceResponse[] prices);
}