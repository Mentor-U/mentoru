using SQLite;

namespace MentorU.Models
{
    public class Users
    {
        [PrimaryKey]
        public string id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        //0 - mentor 1 - mentee 2 - mentor/mentee
        public string Role { get; set; }

        // Move to profiles eventually
        public string Major { get; set; }
        public string Bio { get; set; }

        //public List<string> Classes { get; set; }

    }
}
