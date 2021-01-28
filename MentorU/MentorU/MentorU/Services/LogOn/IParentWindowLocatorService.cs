namespace MentorU.Services.LogOn
{
    /// <summary>
    /// Simple platform specific service that is responsible for locating the parent window
    /// </summary>
    public interface IParentWindowLocatorService
    {
       object GetCurrentParentWindow();
    }
}
