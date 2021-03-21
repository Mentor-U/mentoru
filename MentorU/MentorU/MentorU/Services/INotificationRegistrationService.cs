using System.Threading.Tasks;
using System.Net.Http;

namespace MentorU.Services
{
    public interface INotificationRegistrationService
    {
        Task DeregisterDeviceAsync();
        Task RegisterDeviceAsync(params string[] tags);
        Task RefreshRegistrationAsync();
        Task UpdateTags(string tags);
        Task SendAsync<T>(HttpMethod requestType, string requestUri, T obj);
    }
}
