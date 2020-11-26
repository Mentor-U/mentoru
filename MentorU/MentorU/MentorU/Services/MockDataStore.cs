using MentorU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MentorU.Services
{
    public class MockDataStore : IDataStore
    {
        readonly List<MarketplaceItem> items;
        readonly List<User> Mentors;
        private User _user;

        public MockDataStore()
        {
            _user = new User("Wallace"); // This is the person that is using the app and should be consistent in all aspects of the app
            items = new List<MarketplaceItem>()
            {
                new MarketplaceItem { Id = Guid.NewGuid().ToString(), Text = "First item", ItemPrice = 10.0, Description="This is an item description." },
                new MarketplaceItem { Id = Guid.NewGuid().ToString(), Text = "Second item", ItemPrice = 100.0,Description="This is an item description." }
            };
            Mentors = new List<User>()
            {
                new User("George"),
                new User("Steve")
            };
        }

        /** ---- Marketplace methods ---- */

        public async Task<bool> AddItemAsync(MarketplaceItem item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(MarketplaceItem item)
        {
            var oldItem = items.Where((MarketplaceItem arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((MarketplaceItem arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<MarketplaceItem> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<MarketplaceItem>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }


        /** ---- Profile Methods ---- */
        public async Task<User> GetUser()
        {
            return await Task.FromResult(_user);
        }
        public async Task<bool> UpdateProfileAsync(User user)
        {
            _user = user;
            return await Task.FromResult(true);
        }
        public async Task<IEnumerable<User>> GetMentorsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(Mentors);
        }

    }
}