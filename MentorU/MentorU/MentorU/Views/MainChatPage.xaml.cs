using MentorU.ViewModels;
using MentorU.Views;
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
    public partial class MainChatPage : ContentPage
    {
        private MainChatViewModel _viewModel;
        public MainChatPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new MainChatViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}