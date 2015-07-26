//-----------------------------------------------------------------------
// <copyright file="NotificationManager.cs" company="Sully">
//     Copyright (c) Johnathon Sullinger. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Broadcast
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The message broker for all messaging
    /// </summary>
    public class NotificationManager : INotificationCenter
    {
        /// <summary>
        /// Collection of subscribed listeners
        /// </summary>
        private static ConcurrentDictionary<Type, List<ISubscription>> _listeners =
            new ConcurrentDictionary<Type, List<ISubscription>>();

        /// <summary>
        /// Subscribe publications for the message type specified.
        /// @code
        /// private ISubscription whisperSubscription;
        /// 
        /// public void Initialize()
        /// {
        ///     this.whisperSubscription = notificationManager.Subscribe<WhisperMessage>((msg, sub) => DoStuff);
        /// }
        /// @endcode
        /// </summary>
        /// <typeparam name="TMessageType">A concrete implementation of IMessage</typeparam>
        /// <returns></returns>
        public ISubscription Subscribe<TMessageType>(Action<TMessageType, ISubscription> callback, Func<TMessageType, bool> condition = null) where TMessageType : class, IMessage
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback), "Callback must not be null when subscribing");
            }

            Type messageType = typeof(TMessageType);

            // Create our key if it doesn't exist along with an empty collection as the value.
            if (!_listeners.ContainsKey(messageType))
            {
                _listeners.TryAdd(messageType, new List<ISubscription>());
            }

            // Add our notification to our listener collection so we can publish to it later, then return it.
            // TODO: Move instancing the Notification in to a Factory.
            var handler = new Notification<TMessageType>();
            handler.Register(callback, condition);
            handler.Unsubscribing += this.Unsubscribe;

            List<ISubscription> subscribers = _listeners[messageType];
            lock (subscribers)
            {
                subscribers.Add(handler);
            }

            return handler;
        }

        /// <summary>
        /// Publishes the specified message to all subscribers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        public void Publish<T>(T message) where T : class, IMessage
        {
            foreach (INotification<T> handler in this.GetSubscribersForMessage(message))
            {
                handler.ProcessMessage(message);
            }
        }

        /// <summary>
        /// Publishes the specified message asynchronously to all subscribers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns>Returns an awaitable Task</returns>
        public async Task PublishAsync<T>(T message) where T : class, IMessage
        {
            foreach (INotification<T> handler in this.GetSubscribersForMessage(message))
            {
                await Task.Run(() => handler.ProcessMessage(message));
            }
        }

        private IEnumerable<ISubscription> GetSubscribersForMessage<T>(T message) where T : class, IMessage
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "You can not publish a null message.");
            }

            if (!_listeners.ContainsKey(typeof(T)))
            {
                return Enumerable.Empty<ISubscription>();
            }

            // Create a local reference of the collection to protect us against the collection
            // adding a new subscriber while we're enumerating
            return _listeners[typeof(T)].ToArray();
        }

        /// <summary>
        /// Unsubscribes the specified handler by removing their handler from our collection.
        /// </summary>
        /// <typeparam name="T">The message Type you want to unsubscribe from</typeparam>
        /// <param name="subscription">The subscription to unsubscribe.</param>
        private void Unsubscribe(NotificationArgs args)
        {
            // If the key doesn't exist or has an empty collection we just return.
            // We will leave the key in there for future subscriptions to use.
            if (!_listeners.ContainsKey(args.MessageType) || _listeners[args.MessageType].Count == 0)
            {
                return;
            }

            // Remove the subscription from the collection associated with the key.
            List<ISubscription> subscribers = _listeners[args.MessageType];
            lock (subscribers)
            {
                subscribers.Remove(args.Subscription);
            }

            args.Subscription.Unsubscribing -= this.Unsubscribe;
        }
    }
}
