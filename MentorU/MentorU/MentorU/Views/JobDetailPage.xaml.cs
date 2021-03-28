using MentorU.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MentorU.Models;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobDetailPage : ContentPage
    {
        JobDetailViewModel _vm;
        public JobDetailPage(Jobs j)
        {
            InitializeComponent();
            BindingContext = _vm = new JobDetailViewModel(j);

            ToolbarItem interactButton;

            if (j.Owner.Equals(App.loggedUser.id))
            {
                interactButton = new ToolbarItem
                {
                    Text = "Delete",
                    Command = new Command(_vm.DeleteItem)
                };
            }
            else
            {
                interactButton = new ToolbarItem
                {
                    Text = "Chat",
                    Command = new Command(_vm.StartChat)
                };
            }

            ToolbarItems.Add(interactButton);
        }
    }
}