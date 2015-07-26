//-----------------------------------------------------------------------
// <copyright file="Inspector.cs" company="Sully">
//     Copyright (c) Johnathon Sullinger. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DemoApp
{
    using Broadcast;

    public class Inspector
    {
        private INotificationCenter broker;

        public Inspector(INotificationCenter messageBroker)
        {
            this.broker = messageBroker;
        }

        public string Name { get; set; }

        public void Speak(string message)
        {
            broker.Publish(new BroadcastMessage<InspectorMessage>(new InspectorMessage(message, this)));
        }
    }
}
