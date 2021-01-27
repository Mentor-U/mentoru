using SQLite;
using System.ComponentModel.DataAnnotations;

namespace MentorU.Models
{
    public class Users
    {
        [PrimaryKey]
        public string id { get; set; }
        [Required, System.ComponentModel.DataAnnotations.MaxLength(20)]
        public string FirstName { get; set; }
        [Required, System.ComponentModel.DataAnnotations.MaxLength(20)]
        public string LastName { get; set; }
        [Required, System.ComponentModel.DataAnnotations.MaxLength(20), EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        //0 - mentor 1 - mentee 2 - mentor/mentee
        [Required]
        public string Role { get; set; }

        // Move to profiles eventually
        [Required]
        public string Major { get; set; }
        [Required]
        public string Bio { get; set; }

        //public List<string> Classes { get; set; }

        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}
