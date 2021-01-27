namespace MentorU.Services.LogOn
{
    public static class B2CConstants
    {
        // Azure AD B2C Coordinates
        public static string Tenant = "mentoruauth.onmicrosoft.com";
        public static string AzureADB2CHostname = "mentoruauth.b2clogin.com";
        public static string ClientID = "cc58066e-8c24-4d48-967d-ae1183e382d4";
        public static string PolicySignUpSignIn = "B2C_1_SUSI";
        public static string PolicyEditProfile = "B2C_1_Edit_Profile";
        public static string PolicyResetPassword = "B2C_1_Reset";

        public static string[] Scopes = { "https://mentoruauth.onmicrosoft.com/1095fdb9-aa6e-44dd-9f25-a9fcfa8639cf/demo.read" };

        public static string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public static string AuthoritySignInSignUp = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";
        public static string IOSKeyChainGroup = "com.microsoft.adalcache";
    }
}