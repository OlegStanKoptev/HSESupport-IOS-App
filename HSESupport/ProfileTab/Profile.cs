using System;
namespace HSESupport
{
    public class Profile
    {
        public static Profile Instance { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public int HasPicture { get; set; }
    }
}
