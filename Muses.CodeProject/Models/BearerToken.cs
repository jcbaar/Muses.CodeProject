using Newtonsoft.Json;
using System;

namespace Muses.CodeProject.API.Models
{
    /// <summary>
    /// This object holds a single access token. This can either be a
    /// user access token or a client access token.
    /// </summary>
    public class BearerToken
    {
        /// <summary>
        /// The actual access token string.
        /// </summary>
        [JsonProperty("access_token")]
        public string Token { get; set; }

        /// <summary>
        /// The token type.
        /// </summary>
        [JsonProperty("token_type")]
        public string Type { get; set; }

        /// <summary>
        /// The number of seconds in which the token will
        /// expire.
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// The time stamp at which the token was requested.
        /// </summary>
        public DateTime TokenRequestedAt { get; set; }
    }
}
