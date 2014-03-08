using System;
using System.Collections.Generic;

namespace Broadcast
{
    /// <summary>
    /// NotificationObserver is used by the NotificationManager object.
    /// </summary>
    internal sealed class NotificationObserver
    {
        /// <summary>
        /// Initializes a new instance of the NotificationObserver class.
        /// </summary>
        /// <param name="observer">The object that will be observing for notifications.</param>
        /// <param name="action">The method delegate to invoke upon receiving a notification.</param>
        /// <param name="isAsyncCompatible">If true, the action delegate will be invoked on the calling thread.</param>
        internal NotificationObserver(object observer, Action<object, Dictionary<string, object>> action, bool? isAsyncCompatible)
        {
            this.Observer = observer;
            this.Action = action;
            this.IsAsyncCompatible = isAsyncCompatible;
        }

        /// <summary>
        /// Gets the observer.
        /// </summary>
        internal object Observer { get; private set; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        internal Action<object, Dictionary<string, object>> Action { get; private set; }

        /// <summary>
        /// Gets or sets the is asynchronous compatible.
        /// </summary>
        internal bool? IsAsyncCompatible { get; set; }
    }
}