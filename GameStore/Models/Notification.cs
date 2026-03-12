namespace GameStore.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsRead { get; set; }
    }
}
