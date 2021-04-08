using MentorU.Models;
using MentorU.Services.DatabaseServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            get { return _emailSwitch; }
            set {
                _emailSwitch = value;
                OnPropertyChanged();

            }
        }

        public bool _phoneSwitch;
        public bool phoneSwitch
        {
            get { return _phoneSwitch; }
            set { _phoneSwitch = value; }
        }

        private async void OnSave()
        {

            var usersList = await DatabaseService.Instance.client.GetTable<Settings>().Where(u => u.UserID == App.loggedUser.id).ToListAsync();

            if (usersList.Count == 0)
            {

                Settings newSettings = new Settings()
                {
                    UserID = App.loggedUser.id,
                    EmailSettings = _emailSwitch
                };

                await DatabaseService.Instance.client.GetTable<Settings>().InsertAsync(newSettings);
            }
            else
            {
                var settingsList = await DatabaseService.Instance.client.GetTable<Settings>().Where(u => u.UserID == App.loggedUser.id).ToListAsync();


                JObject data = new JObject
                {
                    {"id",  settingsList[0].id},
                    {"UserID", App.loggedUser.id },
                    {"EmailSettings", _emailSwitch },

                };
                await DatabaseService.Instance.client.GetTable<Settings>().UpdateAsync(data);
            }


            await Application.Current.MainPage.DisplayAlert("Alert", "Settings have been changed.", "Ok");


        }

        public async Task OnAppearing()
        {
            var settingsList = await DatabaseService.Instance.client.GetTable<Settings>().Where(u => u.UserID == App.loggedUser.id).ToListAsync();

            emailSwitch = settingsList.Count > 0 ? settingsList[0].EmailSettings : false;

        }



    }
}