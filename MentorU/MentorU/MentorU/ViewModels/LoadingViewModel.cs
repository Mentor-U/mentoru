
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    class LoadingViewModel : BaseViewModel
    {
      
        public LoadingViewModel()
        {
        }

        // Called by the views OnAppearing method
        public async void Init()
        {
            if (App.loggedUser != null)
            {
                await Shell.Current.GoToAsync("///Main");
            }
            else
            {
                await Shell.Current.GoToAsync("///Login");
            }
        }
    }
}
