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
        private string instName;

        public Inspector(string name)
        {
            instName = name;
            NotificationManager.RegisterObserver(this, "MessageSent", ReceiveMessage);
        }

        public void ReceiveMessage(object sender, Dictionary<string, object> userData)
        {
            if (sender is string)
                Console.WriteLine(instName + " received the following message:" + sender as string);
        }
    }
}
