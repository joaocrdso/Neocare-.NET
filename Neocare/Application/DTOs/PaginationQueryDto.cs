namespace Neocare.Application.DTOs;

public class PaginationQueryDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? OrderBy { get; set; } = "Id";
    public string OrderDirection { get; set; } = "asc";

    public void Validate()
    {
        if (PageNumber < 1) PageNumber = 1;
        if (PageSize < 1) PageSize = 10;
        if (PageSize > 100) PageSize = 100;
        if (string.IsNullOrEmpty(OrderDirection)) OrderDirection = "asc";
    }
}
