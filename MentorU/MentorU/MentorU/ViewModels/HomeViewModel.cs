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
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            GoToProfileCommand = new Command(async () => {
                await Shell.Current.GoToAsync(nameof(ProfilePage));
            });
        }

        public ICommand OpenWebCommand { get; }
        public Command GoToProfileCommand { get; }
    }
}