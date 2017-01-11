using System;
using System.Collections.Generic;

namespace Muses.CodeProject.API.Models
{
    /// <summary>
    /// This class provides information about the user's Total and 
    /// categorized Reputation points and level.
    /// </summary>
    public class Reputation
    {
        /// <summary>
        /// Gets or sets the user's total reputation points.
        /// </summary>
        public int TotalPoints { get; set; }

        /// <summary>
        /// Gets or sets the list of the user's 
        /// <see cref="ReputationType"/> information
        /// </summary>
        public List<ReputationType> ReputationTypes { get; set; }

        /// <summary>
        /// Gets or sets the URL for the user's reputation graph.
        /// </summary>
        public Uri GraphUrl { get; set; }
    }
}
