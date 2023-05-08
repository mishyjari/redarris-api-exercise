namespace RedArrisApi.Models;

public class Return
{
    public string Ticker { get; set; }
    public double ClosePrice { get; set; }
    public DateTime AsOfDate { get; set; }
    public double Value { get; set; }
}