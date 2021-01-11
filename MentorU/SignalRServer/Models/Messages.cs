using System;
namespace SignalRServer.Models
{
    public class Messages
    {
        public int MessagesID { get; set; }
        public string UserID { get; set; }
        public string Text { get; set; }
        public string ChatRoomID { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
