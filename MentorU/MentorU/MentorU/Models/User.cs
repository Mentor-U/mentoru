using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MentorU.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Major { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Classes;

        public User(string name)
        {
            Name = name;
            Major = "Computer Science";
            Bio = "Beep boop bop this is me talking about myself!";
            Classes = new List<string>();
        }
    }
}
