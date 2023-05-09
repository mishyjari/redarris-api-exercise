namespace RedArrisApi;

public interface IPricesService
{
    Task<IEnumerable<IexPriceResponse>> GetPricesAsync(string ticker, string start, string end);
}