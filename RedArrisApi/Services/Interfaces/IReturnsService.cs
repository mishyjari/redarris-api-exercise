namespace RedArrisApi;

public interface IReturnsService
{
    IEnumerable<Return> CalculateReturns(IexPriceDto[] prices);
}