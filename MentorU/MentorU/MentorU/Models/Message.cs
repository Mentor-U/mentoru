using Xamarin.Forms;
﻿

namespace MentorU.Models
{
    public class Message
    {
        public string Text { get; set; }
        public string UserID { get; set; }
        public bool Mine { get; set; }
        public bool Theirs { get; set; }

        //Default values so already used Message objects dont need to be updated (AssitU values) 
        private bool _mentorSearch = false;
        public bool MentorSearch { get => _mentorSearch; set => _mentorSearch = value; }
        private ImageSource _profileImage = null;
        public ImageSource ProfileImage { get => _profileImage; set => _profileImage = value; }
        private string _name = null;
        public string Name { get => _name; set => _name = value; }
        private string id = null;
        public string searchID { get => id; set => id = value; }
    }
}
