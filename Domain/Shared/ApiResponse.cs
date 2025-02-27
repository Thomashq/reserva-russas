namespace Domain.Shared
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public ApiResponse(T data, bool succes, string message, List<string> errors = null) 
        { 
            Data = data;
            Success = succes;
            Message = message;
            Errors = errors;
        }
    }
}
