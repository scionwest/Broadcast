using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Broadcast;
namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Inspector inspector1 = new Inspector("Bob");
            Inspector inspector2 = new Inspector("Jan");

            NotificationManager.RegisterObserver(inspector1, "MessageSent", inspector1.ReceiveMessage);
            NotificationManager.RegisterObserver(inspector2, "MessageSent", inspector2.ReceiveMessage);

            NotificationManager.PostNotification("Program broadcasted to both Inspector's from one Notification Post!", "MessageSent");
            Console.ReadKey();
        }
    }
}
