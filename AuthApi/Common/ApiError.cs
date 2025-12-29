namespace AuthApi.Common
{
    public class ApiError
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
        public string? StackTrace { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
