using System;
using System.Collections.Generic;

namespace Broadcast
{
    internal sealed class NotificationObserver
    {
        internal object Observer { get; private set; }
        internal Action<object, Dictionary<string, object>> Action { get; private set; }

        internal NotificationObserver(object observer, Action<object, Dictionary<string, object>> action)
        {
            this.Observer = observer;
            this.Action = action;
        }
    }
}
