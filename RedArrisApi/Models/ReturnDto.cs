namespace RedArrisApi;

public class ReturnDto
{
    public string Symbol { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public IEnumerable<Return> Returns { get; set; } = new List<Return>();
}