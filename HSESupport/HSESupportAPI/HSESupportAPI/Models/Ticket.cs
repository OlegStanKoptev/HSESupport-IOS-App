namespace HSESupportAPI.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Topic { get; set; }
        public string OpenTime { get; set; }
        public string Status { get; set; }
        public string LastMessageText { get; set; }
        public string LastMessageTime{ get; set; }
        public string FullName { get; set; }
    }
}