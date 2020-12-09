namespace MentorU
{
    /// <summary>
    /// These constants are used in the MSAL library for user authentication with AAD
    /// </summary>
    public static class Constants
    {
        // set your tenant name, for example "contoso123tenant"
        static readonly string tenantName = "mentoruauth";

        // set your tenant id, for example: "contoso123tenant.onmicrosoft.com"
        static readonly string tenantId = "mentoruauth.onmicrosoft.com";

        // set your client/application id, for example: aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
        static readonly string clientId = "65b454ab-7da1-475c-b409-24fc81bf92ca";

        // set your sign up/in policy name, for example: "B2C_1_signupsignin"
        static readonly string policySignin = "B2C_1_SIGN_IN";

        // set your forgot password policy, for example: "B2C_1_passwordreset"
        static readonly string policyPassword = "B2C_1_RESET_PASSWORD";

        // set to a unique value for your app, such as your bundle identifier. Used on iOS to share keychain access.
        // Wasnt sure what to set this to, left it as default from MS docs
        static readonly string iosKeychainSecurityGroup = "com.xamarin.adb2cauthorization";



        // The following fields and properties should not need to be changed
        static readonly string[] scopes = { "" };
        static readonly string authorityBase = $"https://{tenantName}.b2clogin.com/tfp/{tenantId}/";
        public static string ClientId
        {
            get
            {
                return clientId;
            }
        }
        public static string AuthoritySignin
        {
            get
            {
                return $"{authorityBase}{policySignin}";
            }
        }
        public static string AuthorityPasswordReset
        {
            get
            {
                return $"{authorityBase}{policyPassword}";
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
