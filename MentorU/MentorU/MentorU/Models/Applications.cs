using Plugin.FilePicker.Abstractions;
using SQLite;

namespace MentorU.Models
{
    public class Applications
    {
        [PrimaryKey]
        public string id { get; set; }
        public string JobId { get; set; }
        public string ApplicantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Legal { get; set; }
        public string H1B { get; set; }

        [Ignore]
        public FileData Resume { get; set; }
    }
}
