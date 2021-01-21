using Xamarin.Essentials;

namespace MentorU
{
    /// <summary>
    /// These constants are used in the MSAL library for user authentication with AAD.
    /// Im not sure what to do for security/privact of these items.
    /// Im sure they shouldnt be pushed up to git like we have been...
    /// </summary>
    public static class Constants
    {
        //// set your tenant name, for example "contoso123tenant"
        //static readonly string tenantName = "mentoruauth";

        //// set your tenant id, for example: "contoso123tenant.onmicrosoft.com"
        //static readonly string tenantId = "mentoruauth.onmicrosoft.com";

        // set your client/application id, for example: aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
        static readonly string clientId = "0ef84791-fdb9-4e97-ac12-32327cc42644";

        // set to a unique value for your app, such as your bundle identifier. Used on iOS to share keychain access.
        // Wasnt sure what to set this to, left it as default from MS docs
        static readonly string iosKeychainSecurityGroup = "com.xamarin.adb2cauthorization";

        static readonly string[] scopes = { "User.Read" };

        public static string ClientId
        {
            get
            {
                return clientId;
            }
        }

        public static string RedirectUri
        {
            get
            {
                if (DeviceInfo.Platform == DevicePlatform.Android)
                    return $"msauth://{clientId}/{{+J+3yf/mrgPgKeg1llIttpSjcws=}}";
                else if (DeviceInfo.Platform == DevicePlatform.iOS)
                    return $"msauth.{clientId}://auth";

                return string.Empty;
            }
        }

        public static string[] Scopes
        {
            get
            {
                return scopes;
            }
        }

        public static string IosKeychainSecurityGroups
        {
            get
            {
                return iosKeychainSecurityGroup;
            }
        }

    }
}
