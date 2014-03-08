using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Broadcast;
namespace DemoApp
{
    public class Inspector
    {
        public string Name { get; set; }

        public void Speak(string message)
        {
            NotificationManager.PostNotification(this, "InspectorNotification", new Dictionary<string, object> { { "Message", message } });
        }
    }
}
