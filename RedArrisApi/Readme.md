# Returns API

This project is a simple RESTful API that retrieves historical stock price data from the IEX Cloud API and calculates daily returns. The API provides endpoints for retrieving returns for a specified stock symbol within a given date range.

## Setup

* Create a `secrets.json` file in the project root to store your IEX Api Key

```
{
  "IexApiKey": "your-api-key"
}
```
* You can run the project using `dotnet run` or configure your IDE to run using the provided `Properties/runSettngs.json`.
* This project is equipped with **Swagger**, which will be accessible at`https://localhost:7179/swagger/index.html`

## Endpoints

### GET /api/returns/{symbol}

Retrieves daily returns for a given stock symbol.

#### Request parameters

* `symbol` (string): The stock symbol to retrieve returns for.

* `from` (string, optional): The starting date for the return data. Should be in `yyyy-MM-dd` format. If not provided, defaults to first day of the current year.

* `to` (string, optional): The end date for the return data. Should be in `yyyy-MM-dd` format. If not provided, defaults to today's date.

#### Response

Returns a `ReturnDto` object as JSON containing the following properties:

* `Symbol` (string): The stock symbol that the returns are for.

* `FromDate` (DateTime): The starting date of the return data.

* `ToDate` (DateTime): The end date of the return data.

* `Returns` (IEnumerable<Return>): A list of `Return` objects, each containing the following properties:

    * `ClosePrice` (double): The closing price of the stock on the date the return is for.

    * `AsOfDate` (DateTime): The date the return is for.

    * `Value` (double): The daily return of the stock as a percentage, rounded to four decimal places.

## Limitations and Notes

* There is an arbitrary limit to one year of data per request
* If a user provides dates in the query which fail to parse, the request will default to YTD rather than returning a 400
* Returns will be returned as a percentage number to four decimal places, i.e., `2.15` means `2.15%`, rather than experessing the percentage as a decimal, i.e., `.0215`.
* Returns are calculated based on adjusted daily close prices.
  * If requesting a non-existent symbol from IEX, their API returns a 200 with an empty results list, which deserializes successfully into `IexPriceDto`, so this application will handle it with the same behavior as too few results. 

