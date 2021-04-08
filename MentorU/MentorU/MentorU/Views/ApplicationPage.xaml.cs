using MentorU.Models;
using MentorU.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    public partial class ApplicationPage : ContentPage
    {
        public Applications Application { get; set; }
        public ApplicationPage()
        {
            InitializeComponent();
            BindingContext = new ApplicationViewModel();
        }
    }
}