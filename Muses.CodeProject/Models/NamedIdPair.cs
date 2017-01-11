namespace Muses.CodeProject.API.Models
{
    /// <summary>
    /// Class that encapsulates a Name and it's corresponding integer ID.
    /// </summary>
    public class NamedIdPair
    {
        /// <summary>
        /// Gets or sets the Id of the pair.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Name for the pair.
        /// </summary>
        public string Name { get; set; }
    }
}
