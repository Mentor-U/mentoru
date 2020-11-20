using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MentorU.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Major { get; set; }
        public string Bio { get; set; }
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
