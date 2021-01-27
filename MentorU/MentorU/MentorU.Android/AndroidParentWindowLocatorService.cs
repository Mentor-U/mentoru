using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MentorU.Services.LogOn;
using Plugin.CurrentActivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MentorU.Droid
{
    class AndroidParentWindowLocatorService : IParentWindowLocatorService
    {
        public object GetCurrentParentWindow()
        {
            return CrossCurrentActivity.Current.Activity;
        }
    }
}