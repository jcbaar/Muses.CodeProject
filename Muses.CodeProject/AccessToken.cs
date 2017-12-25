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
    public class AccessToken : IDisposable
    {
        private HttpClient _client;
        private BearerToken _userToken;
        private BearerToken _clientToken;
        private string _clientId;
        private string _clientSecret;

        /// <summary>
        /// Constructor. Initializes an instance of the object.
        /// </summary>
        /// <param name="clientId">The client access ID to use when requesting the token.</param>
        /// <param name="clientSecret">The client secret to use when requesting the token.</param>
        public AccessToken(string clientId, string clientSecret)
        {
            Initialize(null, clientId, clientSecret);
        }

        /// <summary>
        /// Constructor. Initializes an instance of the object.
        /// Note: This constructor mainly exists for testing purposes. You normally 
        /// do not need to use this constructor.
        /// </summary>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> for handling the requests.</param>
        /// <param name="clientId">The client access ID to use when requesting the token.</param>
        /// <param name="clientSecret">The client secret to use when requesting the token.</param>
        public AccessToken(HttpMessageHandler handler, string clientId, string clientSecret)
        {
            Initialize(handler, clientId, clientSecret);
        }

        /// <summary>
        /// Initializes the initial state of the object.
        /// </summary>
        /// <exception cref="ArgumentException">Is thrown when either the client ID and/or the client 
        /// secret are null, empty or whitespace.</exception>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> for handling the requests. Note 
        /// that the created HttpClient will take ownership of the handler and dispose of it when 
        /// necessary.</param>
        /// <param name="clientId">The client access ID to use when requesting the token.</param>
        /// <param name="clientSecret">The client secret to use when requesting the token.</param>
        private void Initialize(HttpMessageHandler handler, string clientId, string clientSecret)
        {
            if (String.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("A valid client ID is required.");
            if (String.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentException("A valid client secret is required.");

            _clientId = clientId;
            _clientSecret = clientSecret;
            _client = handler == null ? new HttpClient() : new HttpClient(handler, true);

            _client.BaseAddress = new Uri(Constants.CodeProjectV1ApiUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Helper function to handle the response to a token request.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> to handle.</param>
        /// <returns>The <see cref="BearerToken"/> or null in case of an error.</returns>
        private async Task<BearerToken> HandleResponse(HttpResponseMessage response)
        {
            BearerToken token = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string jsonString = await response.Content.ReadAsStringAsync();

                token = JsonConvert.DeserializeObject<BearerToken>(jsonString);
                token .TokenRequestedAt = DateTime.Now;
                if (!IsValidToken(token))
                {
                    token = null;
                }
            }
            return token;
        }

        /// <summary>
        /// Requests the client or user access token. A request to the API site is only made when
        /// a token was not requested before or, if a token was already requested before, it
        /// has expired.
        /// </summary>
        /// <exception cref="JsonReaderException">This is thrown when the response to the request 
        /// is not valid json.</exception>
        /// <exception cref="HttpRequestException">This is thrown when the request failed.</exception>
        /// <param name="credential">The <see cref="NetworkCredential"/> to use to obtain the user
        /// access token. When this is null a client access token is requested.
        /// <param name="force">Set to true for forcing the HTTP request to get the token.
        /// Defaults to false.</param>
        /// <returns>The user or client access token.</returns>
        public async Task<BearerToken> GetAccessToken(NetworkCredential credential = null, bool force = false)
        {
            var tokenCheck = credential != null ? _userToken : _clientToken;
            if (force || !IsValidToken(tokenCheck))
            {
                try
                {
                    var response = await _client.PostAsync("Token", GetAccessTokenRequestData(credential));
                    if (credential == null)
                    {
                        _clientToken = await HandleResponse(response);
                    }
                    else
                    {
                        _userToken = await HandleResponse(response);
                    }
                }
                catch (HttpRequestException)
                {
                    if (credential == null)
                    {
                        _clientToken = null;
                    }
                    else
                    {
                        _userToken = null;
                    }
                    throw;
                }
            }
            return credential == null ? _clientToken : _userToken;
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
        /// <param name="credential">The <see cref="NetworkCredential"/> with the user credentials
        /// when requesting post data for requesting a user access token. This should be null when
        /// requesting post data for a client access token.</param>
        /// <returns>The <see cref="FormUrlEncodedContent"/> containing the post data.</returns>
        private FormUrlEncodedContent GetAccessTokenRequestData(NetworkCredential credential)
        {
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

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
