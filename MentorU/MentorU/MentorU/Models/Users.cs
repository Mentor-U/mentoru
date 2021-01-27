using SQLite;

namespace MentorU.Models
{
    /// <summary>
    /// Represents a user as far as our application/database is concerned
    /// </summary>
    public class Users
    {
        // Database primary key, we could use the User Identifier from Azure for this?
        [PrimaryKey]
        public string id { get; set; }

        // AAD "Given Name"
        public string FirstName { get; set; }

        // AAD Surname
        public string LastName { get; set; }

        // AAD Display Name
        public string DisplayName { get; set; }

        public string Email { get; set; }

        //0 - mentor 1 - mentee 2 - mentor/mentee
        public string Role { get; set; }

        // Move to profiles eventually
        public string Major { get; set; }
        public string Bio { get; set; }

    }
}
