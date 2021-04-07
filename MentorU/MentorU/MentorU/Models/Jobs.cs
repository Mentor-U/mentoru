using SQLite;
using System;
using Xamarin.Forms;

namespace MentorU.Models
{
    /// <summary>
    /// Represents a Job Listing
    /// </summary>
    public class Jobs
    {
        [PrimaryKey]
        public string id { get; set; }
        public string Text { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Owner { get; set; }
        public string Date { get; set; }

        public string JobType { get; set; }
        public string Level { get; set; }

        [Ignore]
        public ImageSource jobImage { get; set; }
    }
}
