namespace RedArrisApi;

public class ReturnDto
{
    public string Ticker { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Count { get; set; }
    public IEnumerable<Return> Returns { get; set; } = new List<Return>();
}