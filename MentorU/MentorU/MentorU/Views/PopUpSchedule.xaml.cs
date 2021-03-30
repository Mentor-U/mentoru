using System;
using System.Collections.Generic;
using MentorU.ViewModels;
using Xamarin.Forms;

namespace MentorU.Views
{
    public partial class PopUpSchedule
    {
        private ViewOnlyProfileViewModel vm;
        public PopUpSchedule(ViewOnlyProfileViewModel vm)
        {
            InitializeComponent();
            BindingContext = this.vm = vm;
            Animation = new Rg.Plugins.Popup.Animations.ScaleAnimation();
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            ScheduleDate.Text = "I want to schedule a meeting on " + e.NewDate.ToLongDateString() + " at";
            vm._scheduleMessage = ScheduleDate.Text;
        }
    }
}