//-----------------------------------------------------------------------
// <copyright file="MessageBase.cs" company="Sully">
//     Copyright (c) Johnathon Sullinger. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Broadcast
{
    /// <summary>
    /// Provides methods for dispatching notifications to subscription handlers
    /// </summary>
    /// <typeparam name="TMessageType">The type of the message type.</typeparam>
    public abstract class MessageBase<TContentType> : IMessage<TContentType> where TContentType : class
    {
        /// <summary>
        /// Gets the content of the message.
        /// </summary>
        public TContentType Content { get; protected set; }

        /// <summary>
        /// Gets the content of the message.
        /// </summary>
        /// <returns>
        /// Returns the message content
        /// </returns>
        public TContentType GetContent()
        {
            return this.Content;
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <returns>
        /// Returns the content of the message
        /// </returns>
        object IMessage.GetContent()
        {
            return this.GetContent();
        }
    }
}
