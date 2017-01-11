using Muses.CodeProject.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Muses.CodeProject.API
{
    /// <summary>
    /// Class for accessing the CodeProject "Questions API"
    /// </summary>
    public class QuestionsApi : ApiBase
    {
        /// <summary>
        /// Constructor. initializes an instance of the object.
        /// </summary>
        /// <param name="token">The token to use for the API requests. Note that this can
        /// be either a user token or a client token.</param>
        public QuestionsApi(BearerToken token) : base(token)
        {
        }

        /// <summary>
        /// Send a request to get the questions from the API.
        /// </summary>
        /// <param name="mode">The <see cref="QuestionMode"/> for the results.</param>
        /// <param name="include">The tags to include.</param>
        /// <param name="exclude">The tags to exclude.</param>
        /// <param name="page">The page number to request.</param>
        /// <returns>The <see cref="PagedData"/> containing the requested questions.</returns>
        public Task<PagedData> Questions(QuestionMode mode = QuestionMode.New, string include = null, string exclude = null, int page = 1)
        {
            var parameters = new Dictionary<string, string>();
            if (!String.IsNullOrWhiteSpace(include)) parameters.Add("include", include);
            if (!String.IsNullOrWhiteSpace(exclude)) parameters.Add("exclude", include);
            parameters.Add("page", page.ToString());

            return GetRequest<PagedData>(Constants.QuestionsApi_GetQuestions + $"/{mode}" + ToQueryString(parameters));
        }
    }
}
