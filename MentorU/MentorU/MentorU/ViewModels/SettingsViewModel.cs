using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public Command SaveSettingsCommand { get; set; }
        public SettingsViewModel()
        {
            SaveSettingsCommand = new Command(OnSave);
        }
        public bool EmailSwitch
        {
            get{ return EmailSwitch; }
            set{ EmailSwitch = value; }
        }

        private async void OnSave()
        {

        }



    }
}