# Returns API

This project is a simple RESTful API that retrieves historical stock price data from the IEX Cloud API and calculates
daily returns. The API provides an endpoint for retrieving returns for a specified stock symbol within a given date range.

## Setup

* Create a `secrets.json` file in the project root to store your IEX Api Key and the Api key to use as this applications
  token

```
{
  "IexApiKey": "your-iex-api-key",
  "ApiKey": "anything-you-like"
}
```

* You can run the project using `dotnet run` or configure your IDE to run using the
  provided `Properties/runSettings.json`.
* This project is equipped with **Swagger**, which will be accessible at`https://localhost:7179/swagger/index.html`

### Authorization

This application is equipped with a simple api token scheme. This token is defined in the `ApiKey` property in
your `secrets.json` file. It is configued such that the token can be provided as either a query parameter
using `apiKey=yourKey` or as a header value of `X-API-KEY: your-key`.

## Endpoints

### GET /returns/{symbol}

Retrieves daily returns for a given stock symbol.

#### Request parameters

* `symbol` (string): The stock symbol to retrieve returns for.

* `from` (string, optional): The starting date for the return data. Recommended format is `yyyy-MM-dd`, but anything which `DateTime.Parse()` can parse will work. If not provided,
  or if parsing fails, defaults to first day of the current year.

* `to` (string, optional): The end date for the return data. Recommended format is `yyyy-MM-dd`, but anything which `DateTime.Parse()` can parse will work. If not provided,
  or if parsing fails, defaults to today's date.

#### Response

Returns a `ReturnDto` object as JSON containing the following properties:

* `Symbol` (string): The stock symbol that the returns are for.

* `FromDate` (DateTime): The starting date of the return data.

* `ToDate` (DateTime): The end date of the return data.

* `Returns` (IEnumerable<Return>): A list of `Return` objects, each containing the following properties:

    * `ClosePrice` (double): The closing price of the stock on the date the return is for.

    * `AsOfDate` (DateTime): The date the return is for.

    * `Value` (double): The daily return of the stock as a percentage, rounded to four decimal places.

## Services

#### PricesService
This class is responsible for getting historical prices from the IEX API by sending a GET request to the IEX API with the specified ticker, start date, and end date (if not provided by user, these will be passed in as default values).
It uses an HttpClient instance created through the IHttpClientFactory interface to send the request, and then returns the response as an IEnumerable of IexPriceDto objects. 

Note that the request url to IEX includes the `sort=ASC` parameter, so it will not be necessary to sort results elsewhere, however if different data sources or endpoints are used, and the results are not in ascending order by date, **the calculated values for returns will be incorrect**.

If the response is not successful (i.e., it has a status code other than 200), an exception is thrown, which is caught by the controller and returned with a 500 Status code, including the status and message of the failed request to IEX.

#### ReturnsService
This class is responsible for calculating the returns for a given set of prices. It takes an array of IexPriceDto objects as input, and then uses AutoMapper to map each pair of adjacent prices (as Tuples in the order `previous price, next price`) to a Return object.
It then returns an IEnumerable of Return objects, where each element represents the calculated return for the given day, including the date and its close price. The calculation is done in parallel using the Parallel.For method to optimize performance for larger data sets.

#### Note on Calculation
Calculating returns is handled within the AutoMapper profile using the algorithm `Math.Round((currentPrice / previousPrice - 1) * 100, 4)`, which will give us the return value as a whole percentage to four decimal places.

## Tests

This project is bundled with a project each for integration tests and unit tests using MSTest, Moq and FluentAssertions. These should not require any additional configuration, so long as you have set up your `secrets.json' as described above

## Limitations and Notes

* There is an arbitrary limit to one year of data per request
* If a user provides dates in the query which fail to parse, the request will default to YTD rather than returning a 400
* Returns will be returned as a percentage number to four decimal places, i.e., `2.15` means `2.15%`, rather than
  expressing the percentage as a decimal, i.e., `.0215`.
* Returns are calculated based on adjusted daily close prices.
    * If requesting a non-existent symbol from IEX, their API returns a 200 with an empty results list, which
      deserializes successfully into `IexPriceDto`, so this application will handle it with the same behavior as too few
      results. 

## Ideas for imporvement
* Implement API Versioning using `Microsoft.AspNetCore.Mvc.Versioning`
* Allow for users to specify return intervals, i.e., daily, weekly, monthly
* Allow users to specify which price property to calculate returns from, i.e., open, high, low, close
* Implement custom exception handling
* Handle cases for non-existent stock symbol (currently just returns empty price results)
* Add option for how percentage values should be displayed
* Implement custom request validation middleware to clean up the method body of the controller's endpoint

