using System;
namespace HSESupportAPI.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string UserId { get; set; }
        public int MessageId { get; set; }
        public string Type { get; set; }
    }
}
