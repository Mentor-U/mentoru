using MentorU.Services.Identity;
using Splat;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    class LoadingViewModel : BaseViewModel
    {
        private readonly IIdentityService identityService;

        public LoadingViewModel(IIdentityService identityService = null)
        {
            this.identityService = identityService ?? Locator.Current.GetService<IIdentityService>();
        }

        // Called by the views OnAppearing method
        public async void Init()
        {
            var isAuthenticated = await this.identityService.VerifyRegistration();
            if (!isAuthenticated)
            {
                await Shell.Current.GoToAsync("///Login");
            }
        }
    }
}
