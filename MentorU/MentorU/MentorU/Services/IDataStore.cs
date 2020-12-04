using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MentorU.Models;

namespace MentorU.Services
{
    public interface IDataStore
    {
        /** Marketplace*/
        Task<bool> AddItemAsync(MarketplaceItem item);
        Task<bool> UpdateItemAsync(MarketplaceItem item);
        Task<bool> DeleteItemAsync(string id);
        Task<MarketplaceItem> GetItemAsync(string id);
        Task<IEnumerable<MarketplaceItem>> GetItemsAsync(bool forceRefresh = false);

        /** Profile */
        Task<Profile> GetUser(int id = -1);
        Task<IEnumerable<Profile>> GetMentorsAsync(bool forceRefresh = false);
    }
}
