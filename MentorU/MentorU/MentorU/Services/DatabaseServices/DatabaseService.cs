using System;
using System.Threading.Tasks;
using MentorU.Models;
using Microsoft.WindowsAzure.MobileServices;


namespace MentorU.Services.DatabaseServices
{
    /// <summary>
    /// Manage the connection to the database
    /// </summary>
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

        /// <summary>
        /// Check to see if we have record of this user in our database.
        /// Returns TRUE if the user was just created.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> tryCreateAccount(Users user)
        {
            var usersList = await client.GetTable<Users>().Where(u => u.id == user.id).ToListAsync();

            if(usersList.Count > 0) 
            {
                return false;
            }

            await client.GetTable<Users>().InsertAsync(user);
            return true;
            
        }

    }
}
