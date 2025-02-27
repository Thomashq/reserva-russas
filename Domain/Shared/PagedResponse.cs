namespace Domain.Shared
{
    public class PagedResponse<T> : ApiResponse<List<T>>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public PagedResponse(List<T> data, int currentPage, int pageSize, int totalItems, string message = null)
            : base(data, true, message)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalItems;
        }

        //método estático para criar a resposta
        public static PagedResponse<T> Create(List<T> data, int currentPage, int pageSize, int totalItems, string message = null)
        {
            return new PagedResponse<T>(data, currentPage, pageSize, totalItems, message);
        }
    }
}