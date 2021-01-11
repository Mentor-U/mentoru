﻿using MentorU.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MentorU.Services
{
    public class MockDataStore : IDataStore
    {
        readonly List<Items> items;
        readonly List<Users> Mentors;
        private Users _user;

        public MockDataStore()
        {
            _user = new Users { FirstName = "Wallace", Major = "Computer Science", Bio = "I need help with my career path" }; // This is the person that is using the app and should be consistent in all aspects of the app
            //items = new List<Items>()
            //{
            //    new Items { id = Guid.NewGuid().ToString(), Text = "Book: Shakespeare", ItemPrice = "10.0, Description="This is an item description." },
            //    new Items { id = Guid.NewGuid().ToString(), Text = "Computer: Macbook Pro", ItemPrice = "100.0",Description="This is an item description." }
            //};
            Mentors = new List<Users>()
            {
                new Users(){ FirstName = "Jim", Major = "Computer Science" },
                new Users(){ FirstName = "Philip", Major = "Computer Science"},
                new Users() { FirstName = "Bob", Major = "Art", Bio = "Pottery is my favorite" , id="10"},
                new Users() { FirstName = "Jerry", Major = "Comedy", Bio = "I love to make people laugh", id= "11" },
                new Users() { FirstName = "Jonny", Major = "Computer Science", Bio = "I love Machine Learning", id= "12" },

            };
            Mentors[0].id = "1";
            Mentors[1].id = "2";
            Mentors[0].Bio = "I like to ski and I like art";
            Mentors[1].Bio = "I love coffee and code";
        }

        /** ---- Marketplace methods ---- */

        public async Task<bool> AddItemAsync(Items item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Items item)
        {
            var oldItem = items.Where((Items arg) => arg.id == item.id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Items arg) => arg.id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Items> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.id == id));
        }

        public async Task<IEnumerable<Items>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }


        /** ---- Profile Methods ---- */
        public async Task<Users> GetUser(string id = "") // no UserID's should be negative, so -1 can be 'me'
        {   
            if(id == "") // Get the user of the application
                return await Task.FromResult(_user);

            // Get the user data that the primary user wants to interact with
            return await Task.FromResult(Mentors.FirstOrDefault(s => s.id.Equals(id)));
        }
        public async Task<IEnumerable<Users>> GetMentorsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(Mentors);
        }

        public async Task<IEnumerable<Users>> GetAvailableMentors()
        {
            List<Users> available = new List<Users>() {
            new Users() { FirstName = "Bob", Major = "Art", Bio = "Pottery is my favorite", id = "10" },
            new Users() { FirstName = "Jerry", Major = "Comedy", Bio = "I love to make people laugh", id = "11" },
            new Users() { FirstName = "Jonny", Major = "Computer Science", Bio = "I love Machine Learning", id = "12" },
             new Users() { FirstName = "Kramer", Major = "Computer Science", Bio = "I love food", id = "12" }
            };

            return await Task.FromResult(available);
        }

    }
}