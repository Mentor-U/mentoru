

namespace MentorU.Models
{
    public class Message
    {
        public string Text { get; set; }
        public string UserID { get; set; }
        public bool Mine { get; set; }
        public bool Theirs { get; set; }
    }
}
