using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MentorU.Models
{
    public class Users
    {
        [PrimaryKey]
        public string id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        // Move to profiles eventually
        public string Major { get; set; }
        public string Bio { get; set; }

    }
}
