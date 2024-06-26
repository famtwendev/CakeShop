namespace CakeShop.Models
{
    public class ErrorContainer
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
