using System.Collections.Generic;
using System.Threading.Tasks;
using MentorU.Models;

namespace MentorU.Services
{
    public interface IDataStore
    {
        /** Marketplace*/
        Task<bool> AddItemAsync(Items item);
        Task<bool> UpdateItemAsync(Items item);
        Task<bool> DeleteItemAsync(string id);
        Task<Items> GetItemAsync(string id);
        Task<IEnumerable<Items>> GetItemsAsync(bool forceRefresh = false);

        /** Profile */
        Task<Users> GetUser(string id = "");    
        Task<IEnumerable<Users>> GetMentorsAsync(bool forceRefresh = false);
        Task<IEnumerable<Users>> GetAvailableMentors();
    }
}
