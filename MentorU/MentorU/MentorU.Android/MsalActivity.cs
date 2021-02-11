using Android.App;
using Android.Content;
using Microsoft.Identity.Client;


namespace MentorU.Droid
{
    [Activity]
    [IntentFilter(new[] { Intent.ActionView },
      Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
      DataHost = "auth",
      DataScheme = "msalcc58066e-8c24-4d48-967d-ae1183e382d4")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}
