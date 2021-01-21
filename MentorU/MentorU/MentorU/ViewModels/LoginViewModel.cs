using MentorU.Services.Identity;
using Splat;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        private IIdentityService _identityService;

        public LoginViewModel(IIdentityService identityService = null)
        {
            _identityService = identityService ?? Locator.Current.GetService<IIdentityService>();
            ExecuteLogin = new Command(async () => await LoginAsync());
        }

        public ICommand ExecuteLogin { get; set; }

        private async Task LoginAsync()
        {
            await _identityService.VerifyRegistration();
        }

    }
}
