using System;
namespace MentorU.Models
{
    public class BotMessage
    {
        public string ActivityId { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }
        public bool ISent { get; set; }

        //public BotMessage(string activityId, string msg, bool time)
        //{
        //    ActivityId = activityId;
        //    Time = time;
        //    Content = msg;
        //}
    }
}
