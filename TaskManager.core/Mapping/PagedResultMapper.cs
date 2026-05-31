using TaskManager.Core.Dto;

namespace TaskManager.Core.Mapping
{
    public static class PagedResultMapper
    {
        public static PagedResultDto<TDestination> ToPagedDto<TSource, TDestination>(
            this PagedResultDto<TSource> source,
            Func<TSource, TDestination> map)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(map);

            return new PagedResultDto<TDestination>
            {
                Items = source.Items.Select(map).ToList(),
                PageNumber = source.PageNumber,
                PageSize = source.PageSize,
                TotalCount = source.TotalCount
            };
        }
    }
}
