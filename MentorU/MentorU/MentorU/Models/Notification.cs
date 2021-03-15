namespace MentorU.Models
{
    public class Notification
    {
        public string id { get; set; }
        public string MentorID { get; set; }
        public string MenteeID { get; set; }
        public string Message { get; set; }
        public bool Seen { get; set; }
        public bool Unseen { get; set; }
    }
}
