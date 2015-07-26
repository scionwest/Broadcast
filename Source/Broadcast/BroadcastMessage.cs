//-----------------------------------------------------------------------
// <copyright file="BroadcastMessage.cs" company="Sully">
//     Copyright (c) Johnathon Sullinger. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Broadcast
{
    /// <summary>
    /// Provides an implementation of the generic MessageBase class. 
    /// This allows you to pass around content in broadcasts, without having to create a concrete type for each message
    /// if the content of the message is small and does not need any logic.
    /// </summary>
    /// <typeparam name="TContent">The type of the content.</typeparam>
    public class BroadcastMessage<TContent> : MessageBase<TContent> where TContent : class
    {
        public BroadcastMessage(TContent content)
        {
            base.Content = content;
        }
    }
}
