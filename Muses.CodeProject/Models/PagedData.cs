using System.Collections.Generic;

namespace Muses.CodeProject.API.Models
{
    /// <summary>
    /// Class holding the paged result of a request.
    /// </summary>
    public class PagedData
    {
        /// <summary>
        /// Gets or sets the <see cref="Pagination"/> containing information
        /// regarding the paging.
        /// </summary>
        public Pagination Pagination { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="ItemSummary"/> objects for this
        /// page of data.
        /// </summary>
        public List<ItemSummary> Items { get; set; }
    }
}
