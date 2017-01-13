﻿using Muses.CodeProject.API.Models;
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
        /// Requests the client access token. A request to the API site is only made when
        /// a token was not requested before or, if a token was already requested before, it
        /// has expired.
        /// </summary>
        /// <exception cref="JsonReaderException">This is thrown when the response to the request 
        /// is not valid json.</exception>
        /// <exception cref="HttpRequestException">This is thrown when the request failed.</exception>
        /// <param name="force">Set to true for forcing the Http request to get the token.
        /// Defaults to false.</param>
        /// <returns>The client access token.</returns>
        public async Task<BearerToken> GetAccessToken(bool force = false)
        {
            if (force || !IsValidToken(_clientToken))
            {
                try
                {
                    var response = await _client.PostAsync("Token", GetAccessTokenRequestData(null));
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
                catch (HttpRequestException)
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
        /// <exception cref="JsonReaderException">This is thrown when the response to the request 
        /// is not valid json.</exception>
        /// <exception cref="HttpRequestException">This is thrown when the request failed.</exception>
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
                    var response = await _client.PostAsync("Token", GetAccessTokenRequestData(credential));
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
