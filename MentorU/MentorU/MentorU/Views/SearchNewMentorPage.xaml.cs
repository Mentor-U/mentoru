using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MentorU.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchNewMentorPage : ContentPage
    {
        SearchNewMentorViewModel _vm;
        public SearchNewMentorPage()
        {
            InitializeComponent();
            BindingContext = _vm = new SearchNewMentorViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.OnAppearing();
        }
    }
}