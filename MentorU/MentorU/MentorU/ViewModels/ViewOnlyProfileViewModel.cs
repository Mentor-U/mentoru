using MentorU.Models;
using MentorU.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{

    public class ViewOnlyProfileViewModel : BaseViewModel
    {
        private User _user;
        public string Name { get => _user.Name; }
        public string Field { get => _user.Major; }
        public string Bio { get => _user.Bio; }

        public ViewOnlyProfileViewModel()
        {
            _user = new User("George");
            _user.Bio = "I enjoy good coffee and helping people with programming";
        }

    }
}
