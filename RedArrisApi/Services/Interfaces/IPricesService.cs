namespace RedArrisApi;

public interface IPricesService
{
    Task<IEnumerable<IexPriceDto>> GetPricesAsync(string ticker, string from, string to);
}