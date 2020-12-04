using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MentorU.Models
{
    public class Profile : MarketplaceItem
    {
        [PrimaryKey, AutoIncrement]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Major { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<string> Classes;
    }
}
