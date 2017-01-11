using Muses.CodeProject.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Muses.CodeProject.API
{
    public class ApiBase
    {
        private BearerToken _token; // The token to use for the API requests.

        /// <summary>
        /// Constructor. Initializes an instance of the object.
        /// </summary>
        /// <param name="token">The token to use for the API requests</param>
        public ApiBase(BearerToken token)
        {
            RequestToken = token;
            HttpStatusCode = HttpStatusCode.OK;
            HttpStatusMessage = String.Empty;
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
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Constants.CodeProjectV1ApiUrl);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token.Token);

                    HttpResponseMessage response = await client.GetAsync(url);
                    HttpStatusCode = response.StatusCode;
                    HttpStatusMessage = response.ReasonPhrase;

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<T>(jsonString);
                        return responseData;
                    }
                }
            }
            catch (HttpRequestException)
            {
            }
            return default(T);
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
    }
}
