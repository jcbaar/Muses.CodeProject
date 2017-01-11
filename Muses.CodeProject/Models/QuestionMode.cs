namespace Muses.CodeProject.API.Models
{
    /// <summary>
    /// Enumeration for the questions API.
    /// </summary>
    public enum QuestionMode
    {
        /// <summary>
        /// The default mode.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Only get unanswered questions.
        /// </summary>
        Unanswered = 1,

        /// <summary>
        /// Ordered by last activity (descending).
        /// </summary>
        Active = 2,

        /// <summary>
        /// Ordered by date created (descending).
        /// </summary>
        New = 3
    }
}
