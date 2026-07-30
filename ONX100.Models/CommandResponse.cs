
namespace ONX100.Models
{
    public class CommandResponse
    {
        public bool Success { get; set; }

        public string Response { get; set; } = string.Empty;

        public string? ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
