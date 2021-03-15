using System;
using MentorU.Models;

namespace MentorU.Services
{
    public interface IPushNotificationActionService : INotificationActionService
    {
        event EventHandler<PushAction> ActionTriggered;
    }
}
