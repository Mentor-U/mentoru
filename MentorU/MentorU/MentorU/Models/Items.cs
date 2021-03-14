using SQLite;
using Xamarin.Forms;

namespace MentorU.Models
{
    /// <summary>
    /// Represents a Marketplace Item
    /// </summary>
    public class Items
    {
        [PrimaryKey]
        public string id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Owner { get; set; }

        public string ClassUsed { get; set; }
        public string Condition { get; set; }

        [Ignore]
        public ImageSource itemImage { get; set; }
    }
}