using System;

namespace Muses.CodeProject.API.Models
{
    /// <summary>
    /// Class for holding the user profile information.
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// Gets the User ID (UserProfile ID).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or set the user name. Usually a 
        /// 'clean', html stripped, version of DisplayName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the unique (and public) display 
        /// name. Can contain limited HTML
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the url to the user's avatar image.
        /// </summary>
        public Uri Avatar { get; set; }

        /// <summary>
        /// Gets or sets the User's Email Address
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// the user accepts HTML emails.
        /// </summary>
        public bool HtmlEmails { get; set; }

        /// <summary>
        /// Gets or sets the Country Code to be 
        /// displayed for this user.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the URL to the user's 
        /// home page.
        /// </summary>
        public Uri HomePage { get; set; }

        /// <summary>
        /// Gets or sets the user's CodeProject Member Id.
        /// </summary>
        public int CodeProjectMemberId { get; set; }

        /// <summary>
        /// Gets or sets the URL for the User's 
        /// CodeProject Member Profile Page.
        /// </summary>
        public Uri MemberProfilePageUrl { get; set; }

        /// <summary>
        /// Gets or sets the user's Twitter Name.
        /// </summary>
        public string TwitterName { get; set; }

        /// <summary>
        /// Gets or sets the users Google Plus Profile ID.
        /// </summary>
        public string GooglePlusProfile { get; set; }

        /// <summary>
        /// Gets or sets the users LinkedIn Profile ID.
        /// </summary>
        public Uri LinkedInProfileUrl { get; set; }

        /// <summary>
        /// Gets or sets the user's biographical 
        /// information.
        /// </summary>
        public string Biography { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the job title.
        /// </summary>
        public string JobTitle { get; set; }
    }
}
