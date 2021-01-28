using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MentorU.Models;
using MentorU.Services.LogOn;
using Microsoft.WindowsAzure.MobileServices;


namespace MentorU.Services.DatabaseServices
{
    public  class DatabaseService
    {

        public static MobileServiceClient client;


        private  static readonly Lazy<DatabaseService> lazy = new Lazy<DatabaseService>
         (() => new DatabaseService());

        public static DatabaseService Instance { get { return lazy.Value; } }

        private DatabaseService()
        {
            client = new MobileServiceClient("https://mentoruapp.azurewebsites.net");

        }

        

        public async Task<bool> tryCreateAccount(Users user)
        {
            var usersList = await client.GetTable<Users>().Where(u => u.id.Equals(user.id)).ToListAsync();

            if(usersList.Count > 0) 
            {
                return false;
            }

            await client.GetTable<Users>().InsertAsync(user);
            App.loggedUser = user;
            return true;
            
        }

       // public async Task<List<Users>> 

    }
}
