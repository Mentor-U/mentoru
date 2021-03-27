using MentorU.Models;
using MentorU.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    public partial class NewJobPage : ContentPage
    {
        public Jobs Job { get; set; }

        public NewJobPage()
        {
            InitializeComponent();
            BindingContext = new NewJobViewModel();
        }
    }
}