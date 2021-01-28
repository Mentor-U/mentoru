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
            await Shell.Current.GoToAsync("///Login");
        }
    }
}
