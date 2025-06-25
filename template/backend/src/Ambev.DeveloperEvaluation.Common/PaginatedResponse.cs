namespace Ambev.DeveloperEvaluation.Common;

public class PaginatedResponse
{
    public int Page { get; set; }
    public int Size { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}