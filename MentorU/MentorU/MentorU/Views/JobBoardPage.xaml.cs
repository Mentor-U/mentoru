using MentorU.Models;
using MentorU.ViewModels;
using MentorU.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    public partial class JobBoardPage : ContentPage
    {
        JobBoardViewModel _viewModel;

        public JobBoardPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new JobBoardViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}