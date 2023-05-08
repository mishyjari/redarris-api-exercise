using RedArrisApi.Models;

namespace RedArrisApi;

public interface IPricesService
{
    Task<IEnumerable<IexPriceResponse>> GetPricesAsync(string ticker, string? start = null, string? end = null);
}