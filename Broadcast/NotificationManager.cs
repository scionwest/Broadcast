using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Broadcast
{
    /// <summary>
    /// The Notification Manager is responsible for broadcasting notifications to objects that are registered to receive them.
    /// This class is Thread-Safe.
    /// </summary>
    public static class NotificationManager
    {
        /// <summary>
        /// Collection of all observers that have registered to receive a notification.
        /// </summary>
        private static Dictionary<string, List<NotificationObserver>> observers = new Dictionary<string, List<NotificationObserver>>();

        /// <summary>
        /// Gets or sets the SynchronizationContext used for posting synchronous notifications.
        /// If the Context is null, the PostNotification method will broadcast on the calling thread.
        /// If the Context is set, the PostNotification method will broadcast on the Thread associated with the Context.
        /// For setting to the Main UI Thread, it is recommended that this be set from within the MainWindow's constructor.
        /// </summary>
        public static SynchronizationContext Context { get; set; }

        /// <summary>
        /// Registers an object to become a notification observer. 
        /// The object will have the provided method delegate invoked when a object posts the corresponding notification.
        /// </summary>
        /// <param name="observer">The object whom wants to be registered for the Manager to provide notifications to.</param>
        /// <param name="notification">The notification that the observer wants to be told of when they are fired.</param>
        /// <param name="action">The method delegate that will be invoked when the matching notification is posted.</param>
        /// <param name="canRunAsync">If false or null the action method will only be invoked on the Context thread or the calling thread if Context is null. If true the action method will be invoked asynchronously if needed.</param>
        public static void RegisterObserver(object observer, string notification, Action<object, Dictionary<string, object>> action, bool? canRunAsync = null)
        {
            // We only register valid objects.
            if (string.IsNullOrWhiteSpace(notification) || action == null || observer == null)
            {
                return;
            }

            // Create a new NotificationObserver object.
            // Currently you provide it a reference to the observer. This is not used anywhere; there are plans to use this.
            var registeredObserver = new NotificationObserver(observer, action, canRunAsync);

            // Make sure the notification has already been registered.
            // If not, we add the notification to the dictionary, then add the observer.
            if (observers.ContainsKey(notification))
            {
                // Thread-Safe: Do not allow the _Observers property to be modified until we are done with it.
                lock (observers)
                {
                    observers[notification].Add(registeredObserver);
                }
            }
            else
            {
                var observerList = new List<NotificationObserver> { registeredObserver };
                lock (observers)
                {
                    observers.Add(notification, observerList);
                }
            }
        }

        /// <summary>
        /// Unregisters a single observer for the supplied notification.
        /// Since an object can be registered for multiple notifications, you must supply the notification you wish to unregister from.
        /// </summary>
        /// <param name="observer">The object that no longer wants to receive the supplied notification.</param>
        /// <param name="notification">The notification associated with the supplied object.</param>
        public static void UnregisterObserver(object observer, string notification)
        {
            if (observers.ContainsKey(notification))
            {
                var registeredObserver = observers[notification].FirstOrDefault(obs => obs.Observer == observer);

                if (registeredObserver != null)
                {
                    // Since unregistering of an object can happen while a Post is in progress, we attempt to run the unregister process on another thread.
                    // This forces the process to wait until the thread lock is released.
                    // The proper way to address this would be to move everything (posts, registrations and unregistrations) into a queue system 
                    // that runs on a background thread non-stop.
                    Task.Run(() =>
                    {
                        // Thread-Safe: Do not allow _Observers property to be accessd until we are finished.
                        lock (observers)
                        {
                            observers[notification].Remove(registeredObserver);

                            // If we removed the last item in the key's collection, then we remove the key.
                            if (!observers[notification].Any())
                            {
                                observers.Remove(notification);
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Unregisters all observers from posted notifications. 
        /// An optional notification parameter allows for unregistering all objects associated with the supplied notification only.
        /// </summary>
        /// <param name="notification">All observers registered for this notification will become unregistered. All other observed notifications are no affected.</param>
        public static void UnregisterAllObservers(string notification = null)
        {
            if (string.IsNullOrWhiteSpace(notification))
            {
                observers.Clear();
            }
            else
            {
                if (observers.ContainsKey(notification))
                {
                    // Since unregistering of an object can happen while a Post is in progress, we attempt to run the unregister process on another thread.
                    // This forces the process to wait until the thread lock is released.
                    // The proper way to address this would be to move everything (posts, registrations and unregistrations) into a queue system 
                    // that runs on a background thread non-stop.
                    Task.Run(() =>
                    {
                        // Thread-Safe: Do not allow _Observers property to be accessd until we are finished.
                        lock (observers)
                        {
                            observers.Remove(notification);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Posts a notification to the manager for broadcasting to other objects.
        /// When a notification is posted, all objects that observe that notification will be notified via the method delegates they provided when registering.
        /// It is not guaranteed that the observers will have their methods invoked on the current thread.
        /// </summary>
        /// <param name="sender">The sender whom posted the notification.</param>
        /// <param name="notification">The notification being posted.</param>
        /// <param name="userData">Custom data provided by the sender that can be used by all of the observing objects.</param>
        public static void PostNotification(object sender, string notification, Dictionary<string, object> userData = null)
        {
            // Make sure the notification exists.
            if (observers.ContainsKey(notification))
            {
                // Makes sure we never send a null userData dictionary.
                if (userData == null)
                {
                    userData = new Dictionary<string, object>();
                }

                try
                {
                    lock (observers)
                    {
                        // Loop through each objects in the collection and invoke their methods.
                        foreach (NotificationObserver observer in observers[notification].Where(obs => obs != null))
                        {
                            // If Context is null, we fire on what ever thread the caller is on.
                            // If the context is not null, yet the observer is async compatible, we fire on calling thread.
                            if (Context == null ||
                                (observer.IsAsyncCompatible != null && observer.IsAsyncCompatible == true))
                            {
                                observer.Action(sender, userData);
                            }
                            else
                            {
                                // If Context is not null and observer is not async compatible, we fire on Context thread.
                                // Context thread should typically be the Main UI thread.
                                NotificationObserver currentObserver = observer;
                                Context.Send((state) => currentObserver.Action(sender, userData), null);
                            }
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("Attemped to Unregister an observer while a broadcast was taking place. You must wait until the broadcast is completed. If you are unregistering from within a method registered for observation, the unregistration must take place once control has left the registered method.");

                }
                // Clean ourself up.
                Task.Run(() => PurgeNullObservers());
            }
        }

        /// <summary>
        /// Posts a notification to the manager for broadcasting to other objects.
        /// When a notification is posted, all objects that observe that notification will be notified via the method delegates they provided when registering.
        /// This method will invoke all of the object delegate methods asynchronously if the observers allow it.
        /// It is not guaranteed that all observers will run asynchronous if they have registered to run in a forced synchronous fashion.
        /// </summary>
        /// <param name="sender">The sender whom posted the notification.</param>
        /// <param name="notification">The notification being posted.</param>
        /// <param name="userData">Custom data provided by the sender that can be used by all of the observing objects.</param>
        public static void PostNotificationAsync(object sender, string notification, Dictionary<string, object> userData = null)
        {
            // Make sure the notification exists.
            if (observers.ContainsKey(notification))
            {
                // Makes sure we never send a null userData dictionary.
                if (userData == null)
                {
                    userData = new Dictionary<string, object>();
                }

                lock (observers)
                {
                    // Loop through each objects in the collection and invoke their methods.
                    foreach (NotificationObserver observer in observers[notification])
                    {
                        // Some objects can not be fired asynchronous. Such as registered UI elements.
                        if (observer.IsAsyncCompatible != null && observer.IsAsyncCompatible == true)
                        {
                            Task.Run(() => observer.Action(sender, userData));
                        }
                        else
                        {
                            if (Context == null)
                            {
                                observer.Action(sender, userData);
                            }
                            else
                            {
                                Context.Send((state) => observer.Action(sender, userData), null);
                            }
                        }
                    }
                }
            }

            Task.Run(() => PurgeNullObservers());
        }

        /// <summary>
        /// Removes all of the null-referenced observers from the collection. 
        /// This is called asynchronously after every PostNotification, allowing the NotificationManager to be self maintained.
        /// </summary>
        private static void PurgeNullObservers()
        {
            // Thread-Safe: Do not allow _Observers property to be accessd until we are finished.
            lock (observers)
            {
                foreach (KeyValuePair<string, List<NotificationObserver>> notifications in observers)
                {
                    notifications.Value.RemoveAll(predicate => predicate == null || predicate.Observer == null);
                }
            }
        }
    }
}