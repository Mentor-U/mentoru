using System.Threading.Tasks;

namespace MentorU.Services.Identity
{

    interface IIdentityService
    {
        Task<bool> VerifyRegistration();
    }
}
