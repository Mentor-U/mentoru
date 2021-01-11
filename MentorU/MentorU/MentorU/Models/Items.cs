using SQLite;
using System;

namespace MentorU.Models
{
    public class Items
    {
        [PrimaryKey]
        public string id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Owner { get; set; }
    }
}