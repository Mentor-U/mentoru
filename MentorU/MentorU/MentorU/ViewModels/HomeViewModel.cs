using System;
using System.Windows.Input;
using MentorU.Models;
using MentorU.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Title = "Home";
            //OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            GoToProfileCommand = new Command(async () => {
                var profileVM = new ProfileViewModel();
                var profilePage = new ProfilePage
                {
                    BindingContext = profileVM
                };
                await Application.Current.MainPage.Navigation.PushAsync(profilePage);
            });
        }

        public ICommand OpenWebCommand { get; }
        public Command GoToProfileCommand { get; }
    }
}