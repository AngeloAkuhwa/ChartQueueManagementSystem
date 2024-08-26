namespace ChatQueueManagementSystem.Application.Common.Helpers.PaginationHelper
{
	public class PagedResult<TITemType>
	{
		public int ItemCount { get; set; }
		public int PageLength { get; set; }
		public int CurrentPage { get; set; }
		public int PageCount { get; set; }
		public IList<TITemType> Items { get; set; }
	}


	public interface IPagedRequest
	{
		int Page { get; set; }
		int PageLength { get; set; }
	}
}
