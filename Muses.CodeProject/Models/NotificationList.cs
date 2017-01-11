using System.Collections.Generic;

namespace Muses.CodeProject.API.Models
{
    /// <summary>
    /// Class containing a list of notification.
    /// </summary>
    public class NotificationList
    {
        /// <summary>
        /// The list of <see cref="Notification"/> objects.
        /// </summary>
        public List<Notification> Notifications { get; set; }
    }
}
