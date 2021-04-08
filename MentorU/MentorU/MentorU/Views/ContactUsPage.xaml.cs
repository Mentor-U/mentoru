using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactUsPage : ContentPage
    {
        public ContactUsPage()
        {
            InitializeComponent();
        }

        public async void EmailClicked(object sender, EventArgs args)
        {
            await Launcher.OpenAsync((new Uri("mailto:mentoru_support@gmail.com")));
        }

        public async void PhoneClicked(object sender, EventArgs args)
        {
            await Launcher.OpenAsync("tel:18000000000");
        }


    }
}