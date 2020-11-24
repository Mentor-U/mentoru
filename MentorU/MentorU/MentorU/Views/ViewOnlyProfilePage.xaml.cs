﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MentorU.ViewModels;
using MentorU.Views;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewOnlyProfilePage : ContentPage
    {
        public ViewOnlyProfilePage()
        {
            InitializeComponent();
            BindingContext = new ViewOnlyProfileViewModel();
        }
    }
}