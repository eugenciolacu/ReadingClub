namespace ReadingClub.Infrastructure.Common.Paging
{
    public class PagedResponse<T>
    {
        public PagedResponse(IList<T> items, int totalItems) { 
            Items = items;
            TotalItems = totalItems;
        }

        public IList<T> Items { get; } = new List<T>();
        public int TotalItems { get; }
    }
}
