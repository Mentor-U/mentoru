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
            Major = "None";
            Bio = "Hello there.";
            Classes = new List<string>();
        }

        public void AddClass(string newClass)
        {
            Classes.Add(newClass);
        }


    }
}
