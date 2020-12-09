using MentorU.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfilePage : ContentPage
    {
        EditProfileViewModel _viewModel;

        public EditProfilePage(ProfileViewModel profileVM)
        {
            InitializeComponent();
            BindingContext = _viewModel = new EditProfileViewModel(profileVM);
        }
    }
}