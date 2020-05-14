namespace HSESupportAPI.Models
{
    public class Alert
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string TimeCreated { get; set; }
        public string Picture { get; set; }
    }
}