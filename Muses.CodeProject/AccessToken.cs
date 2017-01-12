using Muses.CodeProject.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Muses.CodeProject.API
{
    /// <summary>
    /// Class for requesting and managing client and user access token.
    /// </summary>
    public class AccessToken
    {
        private BearerToken _userToken = null;
        private BearerToken _clientToken = null;
        private string _clientId;
        private string _clientSecret;

        /// <summary>
        /// Constructor. Initializes an instance of the object.
        /// </summary>
        /// <param name="clientId">The client access ID to use when requesting the token.</param>
        /// <param name="clientSecret">The client secret to use when requesting the token.</param>
        public AccessToken(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        /// <summary>
        /// Requests the client access token. A request to the API site is only made when
        /// a token was not requested before or, if a token was already requested before, it
        /// has expired.
        /// </summary>
        /// <param name="force">Set to true for forcing the Http request to get the token.
        /// Defaults to false.</param>
        /// <returns>The client access token.</returns>
        public async Task<BearerToken> GetAccessToken(bool force = false)
        {
            if (force || !IsValidToken(_clientToken))
            {
                try
                {
                    using (var client = GetRequestClient())
                    {
                        var response = await client.PostAsync("Token", GetAccessTokenRequestData(null));
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            string jsonString = await response.Content.ReadAsStringAsync();

                            _clientToken = JsonConvert.DeserializeObject<BearerToken>(jsonString);
                            _clientToken.TokenRequestedAt = DateTime.Now;
                            if (!IsValidToken(_clientToken))
                            {
                                _clientToken = null;
                            }
                        }
                    }
                }
                catch(HttpRequestException)
                {
                    _clientToken = null;
                }
            }
            return _clientToken;
        }

        /// <summary>
        /// Requests the user access token. A request to the API site is only made when
        /// a token was not requested before or, if a token was already requested before, it
        /// has expired.
        /// </summary>
        /// <param name="credential">The <see cref="NetworkCredential"/> to use to obtain the
        /// <param name="force">Set to true for forcing the Http request to get the token.
        /// Defaults to false.</param>
        /// user access token.</param>
        /// <returns>The user access token.</returns>
        public async Task<BearerToken> GetAccessToken(NetworkCredential credential, bool force = false)
        {
            if (force || !IsValidToken(_userToken))
            {
                try
                {
                    using (var client = GetRequestClient())
                    {
                        var response = await client.PostAsync("Token", GetAccessTokenRequestData(credential));
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            string jsonString = await response.Content.ReadAsStringAsync();

                            _userToken = JsonConvert.DeserializeObject<BearerToken>(jsonString);
                            _userToken.TokenRequestedAt = DateTime.Now;
                            if (!IsValidToken(_userToken))
                            {
                                _userToken = null;
                            }
                        }
                        else
                        {
                            _userToken = null;
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    _userToken = null;
                    throw;
                }
            }
            return _userToken;
        }

        /// <summary>
        /// Check to see if the given token is valid and it has not yet expired.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>True for a valid token, false for an invalid token.</returns>
        private bool IsValidToken(BearerToken token)
        {
            if (!String.IsNullOrEmpty(token?.Token) &&
                DateTime.Now.Subtract(token.TokenRequestedAt).Seconds < token.ExpiresIn)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates the post data for requesting either a client access token or
        /// a user access token.
        /// </summary>
        /// <exception cref="InvalidOperationException">Is thrown when either the client ID and/or the client 
        /// secret are null, empty or whitespace.</exception>
        /// <param name="credential">The <see cref="NetworkCredential"/> with the user credentials
        /// when requesting post data for requesting a user access token. This should be null when
        /// requesting post data for a client access token.</param>
        /// <returns>The <see cref="FormUrlEncodedContent"/> containing the post data.</returns>
        private FormUrlEncodedContent GetAccessTokenRequestData(NetworkCredential credential)
        {
            if (String.IsNullOrWhiteSpace(_clientId)) throw new InvalidOperationException("A valid client ID is required.");
            if (String.IsNullOrWhiteSpace(_clientSecret)) throw new InvalidOperationException("A valid client secret is required.");

            // Build up the data to POST.
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();

            if (credential == null)
            {
                postData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            }
            else
            {
                postData.Add(new KeyValuePair<string, string>("grant_type", "password"));
                postData.Add(new KeyValuePair<string, string>("username", credential.UserName));
                postData.Add(new KeyValuePair<string, string>("password", credential.Password));
            }

            postData.Add(new KeyValuePair<string, string>("client_id", _clientId));
            postData.Add(new KeyValuePair<string, string>("client_secret", _clientSecret));

            return new FormUrlEncodedContent(postData);
        }

        /// <summary>
        /// Creates a <see cref="HttpClient"/> ready for requesting the access token.
        /// </summary>
        /// <returns>The created and configured <see cref="HttpClient"/></returns>
        private HttpClient GetRequestClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(Constants.CodeProjectV1ApiUrl);

            // We want the response to be JSON.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
