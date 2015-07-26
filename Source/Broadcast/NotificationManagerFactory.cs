using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadcast
{
    public class NotificationManagerFactory
    {
        private static Func<INotificationCenter> _factory;

        public void SetFactory(Func<INotificationCenter> factory)
        {
            NotificationManagerFactory._factory = factory;
        }

        public static INotificationCenter CreateNotificationCenter()
        {
            return _factory();
        }
    }
}
