using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broadcast
{
    /// <summary>
    /// The Notification Manager is responsible for broadcasting notifications to objects that are registered to receive them.
    /// </summary>
    public static class NotificationManager
    {
        // Collection of notification observers.
        private static Dictionary<string, List<NotificationObserver>> _Observers = new Dictionary<string, List<NotificationObserver>>();

        /// <summary>
        /// Registers an object to become a notification observer. 
        /// The object will have the provided method delegate invoked when a object posts the corresponding notification.
        /// </summary>
        /// <param name="observer">The object whom wants to be registered for the Manager to provide notifications to.</param>
        /// <param name="notification">The notification that the observer wants to be told of when they are fired.</param>
        /// <param name="action">The method delegate that will be invoked when the matching notification is posted.</param>
        public static void RegisterObserver(object observer, string notification, Action<object, Dictionary<string, object>> action)
        {
            // We only register valid objects.
            if (string.IsNullOrWhiteSpace(notification) || action == null || observer == null) return;

            // Create a new NotificationObserver object.
            // Currently you provide it a reference to the observer. This is not used anywhere; there are plans to use this.
            var registeredObserver = new NotificationObserver(observer, action);

            // Make sure the notification has already been registered.
            // If not, we add the notification to the dictionary, then add the observer.
            if (_Observers.ContainsKey(notification))
                _Observers[notification].Add(registeredObserver);
            else
            {
                var observerList = new List<NotificationObserver>();
                observerList.Add(registeredObserver);
                _Observers.Add(notification, observerList);
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
            if (_Observers.ContainsKey(notification))
            {
                var registeredObserver = _Observers[notification].Where(obs => obs.Observer == observer).FirstOrDefault();
                _Observers[notification].Remove(registeredObserver);
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
                _Observers.Clear();
            else
            {
                if (_Observers.ContainsKey(notification))
                {
                    _Observers.Remove(notification);
                }
            }
        }

        /// <summary>
        /// Posts a notification to the manager for broadcasting to other objects.
        /// When a notification is posted, all objects that observe that notification will be notified via the method delegates they provided when registering.
        /// </summary>
        /// <param name="sender">The sender whom posted the notification.</param>
        /// <param name="notification">The notification being posted.</param>
        /// <param name="userData">Custom data provided by the sender that can be used by all of the observing objects.</param>
        public static void PostNotification(object sender, string notification, Dictionary<string, object> userData = null)
        {
            // Make sure the notification exists.
            if (_Observers.ContainsKey(notification))
            {
                // Loop through each objects in the collection and invoke their methods.
                foreach (NotificationObserver observer in _Observers[notification])
                {
                    observer.Action(sender, userData);
                }
            }
        }


        /// <summary>
        /// Posts a notification to the manager for broadcasting to other objects.
        /// When a notification is posted, all objects that observe that notification will be notified via the method delegates they provided when registering.
        /// This method will invoke all of the object delegate methods asynchronously.
        /// </summary>
        /// <param name="sender">The sender whom posted the notification.</param>
        /// <param name="notification">The notification being posted.</param>
        /// <param name="userData">Custom data provided by the sender that can be used by all of the observing objects.</param>
        public static void PostNotificationAsync(object sender, string notification, Dictionary<string, object> userData)
        {
            // Make sure the notification exists.
            if (_Observers.ContainsKey(notification))
            {
                // Loop through each objects in the collection and invoke their methods.
                foreach (NotificationObserver observer in _Observers[notification])
                {
                    Task.Run(new Action(() => { observer.Action(sender, userData); }));
                }
            }
        }
    }
}
