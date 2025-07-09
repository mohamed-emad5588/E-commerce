namespace E_commerce.Wrappers
{
    public class ApiResponse<T>
    {
        public string Status { get; set; } = "success"; // أو "error"
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ApiResponse() { }

        public ApiResponse(T? data, string message = "", string status = "success")
        {
            Data = data;
            Message = message;
            Status = status;
        }
    }

}
