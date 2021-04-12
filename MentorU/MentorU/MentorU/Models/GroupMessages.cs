using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MentorU.Models
{
    public class GroupMessages
    {

        [PrimaryKey]
        public string id { get; set; }

        public string GroupName { get; set; }

        public string Owner { get; set; }
    }
}
