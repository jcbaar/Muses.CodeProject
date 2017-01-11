using System;
using System.Collections.Generic;

namespace Muses.CodeProject.API.Models
{
    /// <summary>
    /// Class representing a single item in the <see cref="PagedData"/> results.
    /// </summary>
    public class ItemSummary
    {
        /// <summary>
        /// Gets or sets the Id of the Item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Title of the Item.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Name/Id pairs of the Item's authors.
        /// </summary>
        public List<NamedIdPair> Authors { get; set; }

        /// <summary>
        /// Gets or sets the Item's Abstract.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the Item's Content Type Name/Id pair.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the Item's Doc Type Name/Id pair.
        /// </summary>
        public NamedIdPair DocType { get; set; }

        /// <summary>
        /// Gets or sets the list of the Item's Category Name/Id pairs.
        /// </summary>
        public List<NamedIdPair> Categories { get; set; }

        /// <summary>
        /// Gets or sets the list of the Item's Tag Name/Id pairs.
        /// </summary>
        public List<NamedIdPair> Tags { get; set; }

        /// <summary>
        /// Gets or sets the Item's License Name/Id pair.
        /// </summary>
        public NamedIdPair License { get; set; }

        /// <summary>
        /// Gets or sets the Item's Creation Date.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the Item's Modification Date.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or set the Name/Id pair for the member who last 
        /// edited this Item's discussion thread, if applicable.
        /// </summary>
        public NamedIdPair ThreadEditor { get; set; }

        /// <summary>
        /// Gets or set the date this Item's discussion thread 
        /// was edited, if applicable.
        /// </summary>
        public DateTime? ThreadModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets this Item's Rating.
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Gets or sets this Item's vote count.
        /// </summary>
        public int Votes { get; set; }

        /// <summary>
        /// Gets or sets this Item's popularity score.
        /// </summary>
        public decimal Popularity { get; set; }

        /// <summary>
        /// Gets or sets the URL to the Item on the website.
        /// </summary>
        public Uri WebsiteLink { get; set; }

        /// <summary>
        /// Gets or sets the link to the full item on the API.
        /// </summary>
        public Uri ApiLink { get; set; }

        /// <summary>
        /// Gets or set the Id of the Parent Message or Article. 
        /// Currently only valid for Messages.
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or set the Id of the Original Message in the discussion thread. 
        /// Currently only valid for Messages.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or set the indent level of the message in the discussion. 
        /// Currently only valid for Messages.
        /// </summary>
        public int IndentLevel { get; set; }
    }
}
