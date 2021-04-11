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
        public JobDetailPage(Jobs j, bool isApplied = false)
        {
            InitializeComponent();
            BindingContext = _vm = new JobDetailViewModel(j);

            ToolbarItem interactButton;

            if (j.Owner.Equals(App.loggedUser.id))
            {
                interactButton = new ToolbarItem
                {
                    Text = "Delete",
                    Command = new Command(_vm.DeleteJob)
                };
            }
            else
            {
                if(!isApplied)
                {
                    interactButton = new ToolbarItem
                    {
                        Text = "Apply",
                        Command = new Command(_vm.StartApply)
                    };
                }
                else
                {
                    interactButton = new ToolbarItem
                    {
                        Text = "Applied"
                    };
                }
            }

            ToolbarItems.Add(interactButton);
        }
    }
}