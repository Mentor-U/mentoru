using System;
namespace MentorU.Models
{
    public class Message
    {
        public string Text { get; set; }
        public Users User { get; set; }
        public bool Mine { get; set; }
        public bool Theirs { get; set; }
    }
}
