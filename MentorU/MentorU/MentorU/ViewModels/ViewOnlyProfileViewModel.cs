﻿using MentorU.Models;
using MentorU.Services.DatabaseServices;
using MentorU.Views.ChatViews;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class ViewOnlyProfileViewModel : BaseViewModel
    {
        private string name;
        private string field;
        private string bio;
        private Users _user;
        private bool _noty;

        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Field { get => field; set => SetProperty(ref field, value); }
        public string Bio { get => bio; set => SetProperty(ref bio, value); }
        public bool FromNotification { get; set; }

        public Command AcceptCommand {get; set;}
        public Command DeclineCommand { get; set; }

        public ViewOnlyProfileViewModel(Users user, bool fromNotification=false)
        {
            _user = user;
            Name = _user.FirstName;
            Field = _user.Major;
            Bio = _user.Bio;
            FromNotification = fromNotification;

            AcceptCommand = new Command(async () => await Accept());
            DeclineCommand = new Command(async () => await Decline());
        }


        /** ------------------------------------------------------
        * Below is the view for users that have not connected 
        * and are looking to connect with them
        * --------------------------------------------------------
        */

        ///<summary>
        /// Sends a request to the mentor requesting a connection be made.
        ///</summary>
        public async void OnRequestMentor()
        {
            try {
                await DatabaseService.Instance.client.GetTable<Notification>().InsertAsync(new Notification()
                {
                    MentorID = _user.id,
                    MenteeID = App.loggedUser.id,
                    Message = $"{App.loggedUser.FirstName} wants to connect!"
                });
                await Application.Current.MainPage.DisplayAlert(
                    "You have sent a request to " + _user.FirstName,
                    "We'll let you know if they accept your request!",
                    "OK");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
         }



        /** --------------------------------------------
        * Below is the view for users that are connected 
        * ----------------------------------------------
        */


        ///<summary>
        /// Opens the chat window with the associated user.
        ///</summary>
        public async void StartChat(object obj)
        {
            await Shell.Current.Navigation.PopToRootAsync(false); // false -> disables navigation animation
            await Shell.Current.Navigation.PushAsync(new ChatPage(_user));
        }

        /// <summary>
        /// Options pane allows users to remove connections that they have.
        /// </summary>
        public async void OpenOptions()
        {
            bool remove = await App.Current.MainPage.DisplayAlert("Options", $"Remove {_user.DisplayName} from your connections?", "Yes", "No");
            if (remove)
            {
                //TODO: need the connection table up again before this will work and then pull the connection instance
                // and delete it from the database.
                //await DatabaseService.client.GetTable<Connection>().DeleteAsync();
            }

        }



        /** ------------------------------------------------------------------------------------
         * Below is the view for users that have not connected and have received a request to
         * from another user 
         * -------------------------------------------------------------------------------------
         */


        async Task Accept()
        {
            bool confirm = await Application.Current.MainPage
                .DisplayAlert("Confirm", $"Do you want to connect with {_user.FirstName}", "Accept", "Cancel");
            if (confirm)
            {
                await DatabaseService.Instance.client.GetTable<Connection>().InsertAsync(new Connection()
                {
                    MentorID = App.loggedUser.id,
                    MenteeID = _user.id
                });

                await App.Current.MainPage.DisplayAlert("Connected!", $"You have connected with {_user.FirstName}", "OK");
                var notification = await DatabaseService.Instance.client.GetTable<Notification>()
                    .Where(u => u.MentorID == App.loggedUser.id && u.MenteeID == _user.id).ToListAsync();
                await DatabaseService.Instance.client.GetTable<Notification>().DeleteAsync(notification[0]);
                await App.Current.MainPage.Navigation.PopModalAsync();
            }
        }

        async Task Decline()
        {
            bool confirm = await Application.Current.MainPage
                .DisplayAlert("Confirm", $"Do you want to decline connection with {_user.FirstName}", "Accept", "Cancel");
            if (confirm)
            {
                var notification = await DatabaseService.Instance.client.GetTable<Notification>()
                    .Where(u => u.MentorID == App.loggedUser.id && u.MenteeID == _user.id).ToListAsync();
                await DatabaseService.Instance.client.GetTable<Notification>().DeleteAsync(notification[0]);
                await App.Current.MainPage
                    .DisplayAlert("Declining Connection.", $"You have declined the connection with {_user.FirstName}", "OK");
                await App.Current.MainPage.Navigation.PopModalAsync();
            }
        }

    }
}
