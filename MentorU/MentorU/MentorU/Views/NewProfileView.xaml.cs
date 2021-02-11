using System;
using System.Collections.Generic;
using MentorU.ViewModels;

using Xamarin.Forms;

namespace MentorU.Views
{
    public partial class NewProfileView : ContentPage
    {
        NewProfileViewModel _vm;

        public NewProfileView()
        {
            InitializeComponent();
            BindingContext = _vm = new NewProfileViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.OnAppearing();
        }
    }
}
