namespace Dorak.DataTransferObject
{
    public class PaginationApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public T Data { get; set; }

        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PaginationApiResponse(bool success, string message, int status, T data, int totalRecords, int currentPage, int pageSize)
        {
            Success = success;
            Message = message;
            Status = status;
            Data = data;
            TotalRecords = totalRecords;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            CurrentPage = currentPage;
        }
    }
}
