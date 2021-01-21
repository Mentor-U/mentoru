using SQLite;

namespace MentorU.Models
{
    public class Users
    {
        // AAD ID 
        [PrimaryKey]
        public string id { get; set; }

        // AAD Given Name
        public string FirstName { get; set; }

        // AAD Surname
        public string LastName { get; set; }

        // AAD Display Name
        public string DisplayName { get; set; }

        // AAD Principal Name, Set up as emails 
        public string PrincipalName { get; set; }


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
