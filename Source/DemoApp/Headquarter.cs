//-----------------------------------------------------------------------
// <copyright file="Headquarter.cs" company="Sully">
//     Copyright (c) Johnathon Sullinger. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DemoApp
{
    using System;
    using Broadcast;

    public class Headquarter : IDisposable
    {
        private ISubscription subscription;

        public Headquarter(INotificationCenter messageBroker)
        {
            // Subscribe to messages broadcasted by any object that will be sending out
            // Inspector instances.
            this.subscription = messageBroker.Subscribe<BroadcastMessage<InspectorMessage>>(
                this.HandleNotification,
                (msg) => msg.Content.Owner.Name != "Bob");
        }

        public void Dispose()
        {
            // Clean up after ourselves.
            this.subscription.Unsubscribe();
        }

        public void HandleNotification(BroadcastMessage<InspectorMessage> message, ISubscription subscription)
        {
            // Wrote the name of the inspector who triggererd this message.
            Console.WriteLine("Inspector " + message.Content.Owner.Name + " says: " + message.Content.Message);
        }
    }
}
