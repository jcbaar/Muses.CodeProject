using Muses.CodeProject.API.Models;
using Muses.CodeProject.Helpers;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;


namespace Muses.CodeProject.API
{
    /// <summary>
    /// Class for accessing the CodeProject "ForumMessages API"
    /// </summary>
    public class ForumMessagesApi : ApiBase
    {
        /// <summary>
        /// Constructor. initializes an instance of the object.
        /// </summary>
        /// <param name="token">The token to use for the API requests. Note that this can
        /// be either a user token or a client token.</param>
        public ForumMessagesApi(BearerToken token) : base(token)
        { }

        /// <summary>
        /// Constructor. Initializes an instance of the object.
        /// </summary>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> for handling the requests.</param>
        /// <param name="token">The token to use for the API requests. Note that this is
        /// to be a user token. Not a client token.</param>
        public ForumMessagesApi(HttpMessageHandler handler, BearerToken token)
            : base(handler, token)
        { }

        /// <summary>
        /// Send a request to get the forum messages from the API.
        /// </summary>
        /// <param name="forumId">The ID of the forum to get the messages from.</param>
        /// <param name="mode">The <see cref="ForumDisplayMode"/> of the messages.</param>
        /// <param name="page">The page number to request.</param>
        /// <returns>The <see cref="PagedData"/> containing the requested messages.</returns>
        public Task<PagedData> Forum(int forumId, ForumDisplayMode mode = ForumDisplayMode.Threads, int page = 1)
        {
            return GetRequest<PagedData>(Constants.ForumApi_GetForum + $"/{forumId}/{mode}?page={page}");
        }

        /// <summary>
        /// Send a request to get the messages from a message thread from the API.
        /// </summary>
        /// <param name="threadId">The ID of the thread to get the messages from.</param>
        /// <param name="page">The page number to request.</param>
        /// <returns>The <see cref="PagedData"/> containing the requested messages.</returns>
        public Task<PagedData> ThreadMessages(int threadId, int page = 1)
        {
            return GetRequest<PagedData>(Constants.ForumApi_GetMessageThread + $"/{threadId}?page={page}");
        }

        /// <summary>
        /// Just a simple stub around the Html scraper that will get us our links.
        /// This will be replaced by a API call just as soon as one becomes available.
        /// </summary>
        /// <param name="page">The page number to request.</param>
        /// <returns>The <see cref="PagedData"/> containing the requested forums.</returns>
        public async Task<PagedData> ListForums(int page = 1)
        {
            PagedData pd = new PagedData();
            pd.Items = await ForumScraperHelper.GetForumLinks();
            pd.Pagination = new Pagination()
            {
                Page = 1,
                PageSize = pd.Items.Count,
                TotalPages = 1,
                TotalItems = pd.Items.Count
            };
            return pd;
        }
    }
}
