using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Broadcast;

namespace DemoApp
{
    public class Headquarter
    {
        public Headquarter()
        {
            NotificationManager.RegisterObserver(this, "InspectorNotification", this.HandleNotification);
        }

        public void HandleNotification(object sender, Dictionary<string, object> userData)
        {
            if (sender is Inspector)
            {
                if (userData.Values.OfType<string>().Any())
                {
                    string message = userData.Values.OfType<string>().FirstOrDefault();
                    Console.WriteLine("Inspector " + (sender as Inspector).Name + " says: " + message);
                }
            }
        }
    }
}
