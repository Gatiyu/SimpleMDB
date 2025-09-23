namespace SimpleMDB;

public class PagedResult<T>
{
    public List<T> Values { get; }
    public int TotalCount { get; }

    public PagedResult(List<T> values, int totalCount)
    {
        Values = values;
        TotalCount = totalCount;
    }
}