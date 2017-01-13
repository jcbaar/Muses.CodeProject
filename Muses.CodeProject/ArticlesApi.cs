using Muses.CodeProject.API.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Muses.CodeProject.API
{
    /// <summary>
    /// Class for accessing the CodeProject "Articles API"
    /// </summary>
    public class ArticlesApi : ApiBase
    {
        /// <summary>
        /// Constructor. initializes an instance of the object.
        /// </summary>
        /// <param name="token">The token to use for the API requests. Note that this can
        /// be either a user token or a client token.</param>
        public ArticlesApi(BearerToken token) : base(token)
        { }

        /// <summary>
        /// Constructor. Initializes an instance of the object.
        /// </summary>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> for handling the requests.</param>
        /// <param name="token">The token to use for the API requests. Note that this is
        /// to be a user token. Not a client token.</param>
        public ArticlesApi(HttpMessageHandler handler, BearerToken token)
            : base(handler, token)
        { }

        /// <summary>
        /// Send a request to get the articles from the API.
        /// </summary>
        /// <param name="tags">The tags for the requested articles. Multiple tags can
        /// be passed by comma separating them.</param>
        /// <param name="minRating">The minimum rating for the articles. Articles rated lower
        /// than this will not be returned.</param>
        /// <param name="page">The page number to request.</param>
        /// <returns>The <see cref="PagedData"/> containing the requested articles.</returns>
        public Task<PagedData> Articles(string tags = null, decimal minRating = 3, int page = 1)
        {
            var parameters = new Dictionary<string, string>();
            if(!String.IsNullOrWhiteSpace(tags)) parameters.Add("tags", tags);
            parameters.Add("minrating", minRating.ToString());
            parameters.Add("page", page.ToString());

            return GetRequest<PagedData>(Constants.ArticlesApi_GetArticles + ToQueryString(parameters));
        }
    }
}
