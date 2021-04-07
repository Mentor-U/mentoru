namespace MentorU.Models
{
    public class Message
    {
        public string Text { get; set; }
        public string UserID { get; set; }
        public bool Mine { get; set; }
        public bool Theirs { get; set; }

        //Default to false so already used Message objects dont need to be updated
        private bool _mentorSearch = false;
        public bool MentorSearch { get => _mentorSearch; set => _mentorSearch = value; }
    }
}
