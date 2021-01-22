using System;
using MentorU.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MentorU.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAccountPage : ContentPage
    {
        public Users newProfile { get; set; }
        public CreateAccountPage()
        {
            InitializeComponent();
            this.BindingContext = new CreateAccountViewModel();
        }
    }
}