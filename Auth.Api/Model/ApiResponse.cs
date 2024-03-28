namespace Auth.Api.Model
{
    public record ApiResponse
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public ErrorResponse? Error { get; set; }
    }
    public record ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }
    }
}
