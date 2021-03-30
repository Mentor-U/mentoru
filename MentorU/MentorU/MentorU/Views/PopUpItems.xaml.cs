using System;
using System.Collections.Generic;
using MentorU.ViewModels;
using Xamarin.Forms;

namespace MentorU.Views
{
    public partial class PopUpItems
    {
        public PopUpItems(ItemsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            Animation = new Rg.Plugins.Popup.Animations.ScaleAnimation();
        }
    }
}