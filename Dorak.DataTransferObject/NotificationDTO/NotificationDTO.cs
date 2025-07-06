namespace Dorak.DataTransferObject
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}