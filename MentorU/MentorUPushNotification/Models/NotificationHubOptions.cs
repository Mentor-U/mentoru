using System;
using System.ComponentModel.DataAnnotations;

namespace MentorUPushNotification.Models
{
    public class NotificationHubOptions
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string ConnectionString { get; set; }
    }
}
