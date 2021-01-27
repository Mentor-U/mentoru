using MentorU.Services.LogOn;
using Plugin.CurrentActivity;

namespace MentorU.Android
{
    class AndroidParentWindowLocatorService : IParentWindowLocatorService
    {
        public object GetCurrentParentWindow()
        {
            return CrossCurrentActivity.Current.Activity;
        }
    }
}