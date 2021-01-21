using Android.App;
using Android.Content;
using Microsoft.Identity.Client;


namespace MentorU.Droid
{
    [Activity]
    [IntentFilter(new[] { Intent.ActionView },
      Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
      DataHost = "auth",
      DataScheme = "msal0ef84791-fdb9-4e97-ac12-32327cc42644")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}
