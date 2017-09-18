using Muses.CodeProject.API.Models;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace Muses.CodeProject.API
{
    /// <summary>
    /// Class for accessing the CodeProject "My API"
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MyApi : ApiBase
    {
        /// <summary>
        /// Constructor. Initializes an instance of the object.
        /// </summary>
        /// <param name="token">The token to use for the API requests. Note that this is
        /// to be a user token. Not a client token.</param>
        public MyApi(BearerToken token) : base(token)
        { }

        /// <summary>
        /// Constructor. Initializes an instance of the object.
        /// </summary>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> for handling the requests.</param>
        /// <param name="token">The token to use for the API requests. Note that this is
        /// to be a user token. Not a client token.</param>
        public MyApi(HttpMessageHandler handler, BearerToken token)
            : base(handler, token)
        { }

        /// <summary>
        /// Request the <see cref="UserProfile"/> belonging to the request token.
        /// </summary>
        /// <returns>The <see cref="UserProfile"/> or null in case of an error.</returns>
        public Task<UserProfile> GetUserProfileAsync() => GetRequest<UserProfile>(Constants.MyApi_Profile);

        /// <summary>
        /// Request the <see cref="Reputation"/> belonging to the request token.
        /// </summary>
        /// <returns>The <see cref="Reputation"/> or null in case of an error.</returns>
        public Task<Reputation> GetReputationAsync() => GetRequest<Reputation>(Constants.MyApi_Reputation);

        /// <summary>
        /// Request the <see cref="NotificationList"/> belonging to the request token.
        /// </summary>
        /// <returns>The <see cref="NotificationList"/> or null in case of an error.</returns>
        public Task<NotificationList> GetNotificationsAsync() => GetRequest<NotificationList>(Constants.MyApi_Notifications);

        /// <summary>
        /// Request the <see cref="PagedData"/> for the answers belonging to the request token.
        /// </summary>
        /// <returns>The <see cref="PagedData"/> or null in case of an error.</returns>
        public Task<PagedData> GetAnswersAsync(int pageNr = 1) => GetPagedData(Constants.MyApi_Answers, pageNr);

        /// <summary>
        /// Request the <see cref="PagedData"/> for the articles belonging to the request token.
        /// </summary>
        /// <returns>The <see cref="PagedData"/> or null in case of an error.</returns>
        public Task<PagedData> GetArticlesAsync(int pageNr = 1) => GetPagedData(Constants.MyApi_Articles, pageNr);

        /// <summary>
        /// Request the <see cref="PagedData"/> for the blog posts belonging to the request token.
        /// </summary>
        /// <returns>The <see cref="PagedData"/> or null in case of an error.</returns>
        public Task<PagedData> GetBlogPostsAsync(int pageNr = 1) => GetPagedData(Constants.MyApi_BlogPosts, pageNr);

        /// <summary>
        /// Request the <see cref="PagedData"/> for the bookmarks belonging to the request token.
        /// </summary>
        /// <returns>The <see cref="PagedData"/> or null in case of an error.</returns>
        public Task<PagedData> GetBookmarksAsync(int pageNr = 1) => GetPagedData(Constants.MyApi_Bookmarks, pageNr);

        /// <summary>
        /// Request the <see cref="PagedData"/> for the messages belonging to the request token.
        /// </summary>
        /// <returns>The <see cref="PagedData"/> or null in case of an error.</returns>
        public Task<PagedData> GetMessagesAsync(int pageNr = 1) => GetPagedData(Constants.MyApi_Messages, pageNr);

        /// <summary>
        /// Request the <see cref="PagedData"/> for the questions belonging to the request token.
        /// </summary>
        /// <returns>The <see cref="PagedData"/> or null in case of an error.</returns>
        public Task<PagedData> GetQuestionsAsync(int pageNr = 1) => GetPagedData(Constants.MyApi_Questions, pageNr);

        /// <summary>
        /// Request the <see cref="PagedData"/> for the tips belonging to the request token.
        /// </summary>
        /// <returns>The <see cref="PagedData"/> or null in case of an error.</returns>
        public Task<PagedData> GetTipsAsync(int pageNr = 1) => GetPagedData(Constants.MyApi_Tips, pageNr);
    }
}
