namespace Aftab.Shared.Application.Pagination;

public class PaginatedResult<TEntity>
    (int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
    where TEntity : class
{
    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public long Count { get; } = count;
#pragma warning disable S125
    // public int PageCount { get; init; } = Convert.ToInt32(Math.Ceil(Count/(double)pageSize));
#pragma warning restore S125
    public IEnumerable<TEntity> Data { get; } = data;
}
