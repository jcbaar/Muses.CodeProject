using System.Diagnostics.CodeAnalysis;

namespace Muses.CodeProject.API
{
    /// <summary>
    /// Constant strings used by the API classes.
    /// TODO: Move them into the API specific classes?
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class Constants
    {
        // CodeProject API base URL.
        public static string CodeProjectV1ApiUrl => "https://api.codeproject.com";

        // My API command URL's
        public static string MyApi_Profile  => "v1/My/Profile";
        public static string MyApi_Reputation => "v1/My/Reputation";
        public static string MyApi_Notifications => "v1/My/Notifications";
        public static string MyApi_Answers => "v1/My/Answers";
        public static string MyApi_Articles => "v1/My/Articles";
        public static string MyApi_BlogPosts => "v1/My/BlogPosts";
        public static string MyApi_Bookmarks => "v1/My/Bookmarks";
        public static string MyApi_Messages => "v1/My/Messages";
        public static string MyApi_Questions => "v1/My/Questions";
        public static string MyApi_Tips => "v1/My/Tips";

        // Article API command URL's
        public static string ArticlesApi_GetArticles => "v1/Articles";

        // Questions API command URL's
        public static string QuestionsApi_GetQuestions => "v1/Questions";

        // ForumMessages API command URL's
        public static string ForumApi_GetForum => "v1/Forum";
        public static string ForumApi_GetMessageThread => "v1/MessageThread";
    }
}
