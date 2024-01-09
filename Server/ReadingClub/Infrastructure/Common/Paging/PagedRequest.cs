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
        public string OrderBy { get; set; } = string.Empty;
        public string OrderDirection { get; set; } = string.Empty;
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
        public string? UserEmail { get; set; }
    }
}
