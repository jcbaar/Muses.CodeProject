using Muses.CodeProject.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Muses.CodeProject.API
{
    /// <summary>
    /// Base class for the differen API classes. This class handles communication
    /// with the codeproject API. Internally this class uses the <see cref="HttpClient"/>
    /// class and therefore has the IDisposable interface implemented.
    /// 
    /// Note however that the same rules apply for the API classes as for the <see cref="HttpClient"/>
    /// class. Normally you will only need to instantiate an API class once and use that
    /// throughout the application lifetime instead of instantiating one for each call
    /// to the API.
    /// </summary>
    public class ApiBase : IDisposable
    {
        private BearerToken _token; // The token to use for the API requests.
        private static HttpClient _client; // The HttpClient to use for the requests.
        private static int _clientCount; // The HttpClient is reference counted.
        private static object _lock = new object(); // Synchronized access for the static fields.

        /// <summary>
        /// Constructor. Initializes an instance of the object.
        /// </summary>
        /// <param name="token">The token to use for the API requests</param>
        public ApiBase(BearerToken token)
        {
            Initialize(null, token);
        }

        /// <summary>
        /// Constructor. Initializes an instance of the object.
        /// Note: This constructor mainly exists for testing purposes. You normally 
        /// do not need to use this constructor.
        /// </summary>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> for handling the requests.</param>
        /// <param name="token">The token to use for the API requests</param>
        public ApiBase(HttpMessageHandler handler, BearerToken token)
        {
            Initialize(handler, token);
        }

        /// <summary>
        /// Initializes the initial state of the object.
        /// </summary>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> for handling the requests. Note 
        /// that the created HttpClient will take ownership of the handler and dispose of it when 
        /// necessary.</param>
        /// <exception cref="InvalidOperationException">This is thrown when the token is either null or the
        /// token value is null or empty.</exception>
        /// <param name="token">The token to use for the API requests</param>
        private void Initialize(HttpMessageHandler handler, BearerToken token)
        {
            if (token == null || String.IsNullOrWhiteSpace(token.Token))
            {
                throw new InvalidOperationException("Token value must have a valid contents.");
            }

            HttpStatusCode = HttpStatusCode.OK;
            HttpStatusMessage = HttpStatusCode.ToString();

            lock (_lock)
            {
                if (_client == null)
                {
                    _client = handler == null ? new HttpClient() : new HttpClient(handler, true);
                    _client.BaseAddress = new Uri(Constants.CodeProjectV1ApiUrl);
                }

                // The HttpClient instance is created once and used for all the remaining
                // ApiBase instances created. Therefore the instantiation is reference counted
                // so that we only dispose of the HttpClient once the last ApiBase instance
                // is disposed of.
                Interlocked.Add(ref _clientCount, 1);
                RequestToken = token;
            }
        }

        /// <summary>
        /// (Re-)sets the headers for the HttpClient.
        /// </summary>
        private void SetClientHeaders()
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + RequestToken.Token);
        }

        /// <summary>
        /// Gets or sets the <see cref="BearerToken"/> to use for the next request(s).
        /// </summary>
        public BearerToken RequestToken
        {
            get
            {
                return _token;
            }

            set
            {
                if(value == null || String.IsNullOrWhiteSpace(value.Token))
                {
                    throw new InvalidOperationException("Token value must have a valid contents.");
                }
                _token = value;
                lock (_lock)
                {
                    SetClientHeaders();
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="HttpStatusCode"/> of the last request. 
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; protected set; }

        /// <summary>
        /// Gets the status message of the last request.
        /// </summary>
        public string HttpStatusMessage { get; protected set; }

        /// <summary>
        /// Converts a <see cref="NameValueCollection"/> to a query parameter string.
        /// <see cref="http://stackoverflow.com/questions/829080/how-to-build-a-query-string-for-a-url-in-c"/>
        /// </summary>
        /// <param name="nvc">The <see cref="Dictionary<string, string>"/> to convert to a query
        /// parameter string.</param>
        /// <returns>The query parameter string.</returns>
        protected string ToQueryString(Dictionary<string, string> nvc)
        {
            var array = (from key in nvc.Keys
                         from value in nvc.Values
                         select string.Format("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }

        /// <summary>
        /// Sends a request to the API and returns the response to the request.
        /// </summary>
        /// <typeparam name="T">The type of response that is expected to be returned.</typeparam>
        /// <param name="url">The relative API URL to call.</param>
        /// <returns>An object of type T with the response data or null when an exception was thrown or
        /// a non-OK response was returned.</returns>
        protected async Task<T> GetRequest<T>(string url) where T : new()
        {
            using (var response = await _client.GetAsync(url))
            {
                HttpStatusCode = response.StatusCode;
                HttpStatusMessage = response.ReasonPhrase;

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrWhiteSpace(jsonString))
                    {
                        var responseData = JsonConvert.DeserializeObject<T>(jsonString);
                        return responseData;
                    }
                }
                return default(T);
            }
        }

        /// <summary>
        /// Helper method for sending a request that expects a result of
        /// <see cref="PagedData"/>
        /// </summary>
        /// <param name="dataUrl">The API URL to call.</param>
        /// <param name="pageNr">The number of the page of data to request.</param>
        /// <returns>The response data.</returns>
        protected async Task<PagedData> GetPagedData(string dataUrl, int pageNr)
        {
            return await GetRequest<PagedData>($"{dataUrl}?page={pageNr}");
        }

        #region IDisposable Support
        /// <summary>
        /// Gets the disposed state of the ApiBase HttpClient. When there are still
        /// un-disposed ApiBase instances present this will return false. Once all
        /// ApiBase instances are properly disposed of (or no instances where yet
        /// created) this will return true.
        /// </summary>
        public static bool IsDisposed
        {
            get
            {
                return Interlocked.CompareExchange(ref _clientCount, 0, 0) == 0;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposing)
            {
                return;
            }

            // Only really dispose of the HttpClient when the reference count
            // reaches 0.
            int current = Interlocked.Decrement(ref _clientCount);
            if(current > 0)
            {
                return;
            }

            lock (_lock)
            {
                // The null-conditional operator is probably being a bit paranoid
                // since the _client variable can not be null at this point...
                _client?.Dispose();
                _client = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
