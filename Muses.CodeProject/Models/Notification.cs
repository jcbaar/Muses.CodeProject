using System;

namespace Muses.CodeProject.API.Models
{
    /// <summary>
    /// Class holding a single notification.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Gets or sets the id of the notification.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the type of item the 
        /// notification is about.
        /// </summary>
        public string ObjectTypeName { get; set; }

        /// <summary>
        /// Gets or sets the Id of the Object being 
        /// reported about.
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the subject (title) of the 
        /// notification.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the topic of the notification.
        /// </summary>
        public string Topic{ get; set; }

        /// <summary>
        /// Gets or sets the string form of the date 
        /// when notification was received.
        /// </summary>
        public DateTime NotificationDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// the message is unread.
        /// </summary>
        public bool UnRead { get; set; }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the URL to go to the item.
        /// </summary>
        public Uri Link { get; set; }
    }
}
