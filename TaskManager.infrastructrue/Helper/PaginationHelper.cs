using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Dto;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Helper
{
    public static class PaginationHelper
    {
        private const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 50;

        public static async Task<PagedResultDto<T>> ToPagedResultAsync<T>(IQueryable<T> query, int pageNumber, int pageSize) where T : BaseEntity
        {
            var normalizedPageNumber = pageNumber < 1 ? DefaultPageNumber : pageNumber;
            var normalizedPageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

            var skip = (normalizedPageNumber - 1) * normalizedPageSize;
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Id)
                .Skip(skip)
                .Take(normalizedPageSize)
                .ToListAsync();

            return new PagedResultDto<T>
            {
                Items = items,
                PageNumber = normalizedPageNumber,
                PageSize = normalizedPageSize,
                TotalCount = totalCount
            };
        }
    }
}
