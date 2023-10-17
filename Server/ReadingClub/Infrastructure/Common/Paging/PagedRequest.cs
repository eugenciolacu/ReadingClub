namespace ReadingClub.Infrastructure.Common.Paging
{
    public class PagedRequest
    {
        public PagedRequest()
        {
        }

        public PagedRequest(int pageSize, int page, string orderBy, string orderDirection, Dictionary<string, string> filters, string? userEmail = null) 
        {
            PageSize = pageSize;
            Page = page;
            OrderBy = orderBy;
            OrderDirection = orderDirection;
            Filters = filters;
            UserEmail = userEmail;
        }

        public int PageSize { get; set; }
        public int Page { get; set; }
        public string OrderBy { get; set; }
        public string OrderDirection { get; set; }
        public Dictionary<string, string> Filters { get; set; }
        public string? UserEmail { get; set; }
    }
}
