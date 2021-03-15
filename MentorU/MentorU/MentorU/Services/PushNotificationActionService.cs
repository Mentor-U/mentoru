using System;
using System.Collections.Generic;
using System.Linq;
using MentorU.Models;

namespace MentorU.Services
{
    public class PushNotificationActionService : IPushNotificationActionService
    {
        readonly Dictionary<string, PushAction> _actionMappings = new Dictionary<string, PushAction>
        {
            { "action_a", PushAction.ActionA },
            { "action_b", PushAction.ActionB }
        };

        public event EventHandler<PushAction> ActionTriggered = delegate { };

        public void TriggerAction(string action)
        {
            if (!_actionMappings.TryGetValue(action, out var pushDemoAction))
                return;

            List<Exception> exceptions = new List<Exception>();

            foreach (var handler in ActionTriggered?.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(this, pushDemoAction);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }
    }
}
