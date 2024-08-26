using Microsoft.EntityFrameworkCore;

namespace ChatQueueManagementSystem.Application.Common.Helpers.PaginationHelper
{
	public static class PagedQueryHelper
	{
		public static Task<PagedResult<T>> ToPagedResultsAsync<T>(this IQueryable<T> queryable, IPagedRequest request, int maxLength = 50)
		{
			var pageLength = request.PageLength > maxLength ? maxLength : request.PageLength;

			return queryable.ToPagedResultsAsync<T, PagedResult<T>>(request.Page, pageLength);
		}

		private static async Task<TResponse> ToPagedResultsAsync<T, TResponse>(this IQueryable<T> queryable, int page, int pageLength)
			where TResponse : PagedResult<T>, new()
		{
			page = page < 1 ? 1 : page;
			pageLength = pageLength < 1 ? 10 : pageLength;
			var items = await queryable.Page(page, pageLength).ToListAsync();
			var count = await queryable.CountAsync();
			var pageCount = (int)Math.Ceiling(count / (double)pageLength);

			return new TResponse
			{
				ItemCount = count,
				PageLength = pageLength,
				CurrentPage = page,
				PageCount = pageCount,
				Items = items
			};
		}

		private static IQueryable<T> Page<T>(this IQueryable<T> queryable, int pageIndex, int pageLength, bool zeros = false)
		{
			if (!zeros)
			{
				pageIndex -= 1;
			}

			var itemsToSkip = Math.Max(pageIndex * pageLength, 0);
			return queryable.Skip(itemsToSkip).Take(pageLength);
		}
	}
}
