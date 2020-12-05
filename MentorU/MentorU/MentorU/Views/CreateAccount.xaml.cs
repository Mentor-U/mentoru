using MentorU.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MentorU.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;

namespace MentorU.Views
{

    public partial class CreateAccount : ContentPage
    {
        public Users newProfile { get; set; }
        public CreateAccount()
        {
            InitializeComponent();
            BindingContext = new CreateAccountViewModel();
        }

    }
}