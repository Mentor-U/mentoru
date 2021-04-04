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
        public bool _emailSwitch;
        public bool emailSwitch
        {
            get{ return _emailSwitch; }
            set{ _emailSwitch = value; }
        }

        public bool _phoneSwitch;
        public bool phoneSwitch
        {
            get { return _phoneSwitch; }
            set { _phoneSwitch = value; }
        }

        private async void OnSave()
        {

        }



    }
}