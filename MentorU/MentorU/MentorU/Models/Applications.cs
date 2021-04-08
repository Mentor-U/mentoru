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

        [Ignore]
        public FileData Resume { get; set; }
    }
}
