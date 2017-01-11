namespace Muses.CodeProject.API.Models
{
    /// <summary>
    /// This class provides information about the user's reputation in a particular Reputation Type.
    /// </summary>
    public class ReputationType
    {
        /// <summary>
        /// Gets or sets the Reputation Type Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user's Points for this 
        /// Reputation Type.
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the name of the Level achieved 
        /// by the user in this Reputation Type.
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets the Designation the user 
        /// can use for this achievement.
        /// </summary>
        public string Designation { get; set; }
    }
}
