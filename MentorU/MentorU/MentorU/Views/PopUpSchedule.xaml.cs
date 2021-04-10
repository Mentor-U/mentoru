using System;
using System.Collections.Generic;
using MentorU.Models;
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

        private async void OnTextChanged(object sender, EventArgs eventArgs)
        {
            if (!string.IsNullOrWhiteSpace(vm.AddressText))
            {
                await vm.GetPlacesPredictionsAsync();
            }
        }

        private void AddressList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var obj = (AddressInfo) e.SelectedItem;
            var add = obj.Address;
            vm.AddressText = add;
        }
    }
}