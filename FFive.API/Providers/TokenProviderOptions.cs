using Microsoft.IdentityModel.Tokens;
using System;

namespace FFive.API.Providers
{
    public class TokenProviderOptions
    {
        /// The relative request path to listen on. The default path is /token.
        public string Path { get; set; } = "api/token";

        ///  The Issuer (iss) claim for generated tokens.
        public string Issuer { get; set; }

        /// The Audience (aud) claim for the generated tokens.
        public string Audience { get; set; }

        /// The expiration time for the generated tokens.
        /// The default is five minutes (300 seconds).
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(60);

        /// The signing key to use when generating tokens.
        public SigningCredentials SigningCredentials { get; set; }
    }
}