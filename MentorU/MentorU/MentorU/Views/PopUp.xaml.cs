using System;
using System.Collections.Generic;
using MentorU.ViewModels;
using Xamarin.Forms;

namespace MentorU.Views
{
    public partial class PopUp 
    {
        public PopUp(SearchNewMentorViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            Animation = new Rg.Plugins.Popup.Animations.ScaleAnimation();
        }
    }
}
