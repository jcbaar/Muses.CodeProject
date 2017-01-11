namespace Muses.CodeProject.API.Models
{
    /// <summary>
    /// Class holding paging information.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the size of a whole page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of
        /// available pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total number of 
        /// available items.
        /// </summary>
        public int TotalItems { get; set; }
    }    
}
