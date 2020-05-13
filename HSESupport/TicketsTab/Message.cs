using System;
namespace HSESupport.TicketsTab
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Sender { get; set; }
        public string SendTime { get; set; }
        public string UserId { get; set; }
        public int TicketId { get; set; }
        public string Type { get; set; }
    }
}
