using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Fixtures;

namespace Broadcast.Tests
{
    [TestClass]
    public class NotificationManagerTests
    {
        [TestMethod]
        [TestCategory("Runtime.Game - NotificationManager")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Subscribe_with_null_calback_throws_exception()
        {
            // Arrange
            var notificationCenter = new NotificationManager();

            // Act
            notificationCenter.Subscribe<BroadcastMessage<SimpleContent>>(null);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Runtime.Game - NotificationManager")]
        public void Publish_invokes_callbacks()
        {
            bool callbackCalled = false;
            string messageContent = "Test";
            var notificationCenter = new NotificationManager();
            ISubscription subscription = notificationCenter.Subscribe<BroadcastMessage<SimpleContent>>(
                (msg, sub) =>  callbackCalled = msg.Content.Name == messageContent);

            // Act
            notificationCenter.Publish(new BroadcastMessage<SimpleContent>(new SimpleContent { Name = messageContent }));

            // Assert
            Assert.IsTrue(callbackCalled, "The subscriber did not have its callback invoked.");
        }

        [TestMethod]
        [TestCategory("Runtime.Game - NotificationManager")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Publish_with_null_message_throws_exception()
        {
            var notificationCenter = new NotificationManager();
            notificationCenter.Subscribe<BroadcastMessage<SimpleContent>>((msg, sub) => { });

            // Act
            notificationCenter.Publish<BroadcastMessage<SimpleContent>>(null);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Runtime.Game - NotificationManager")]
        public void Handler_can_unsubscribe()
        {
            var notificationCenter = new NotificationManager();
            int callCount = 0;

            // Build our notification.
            ISubscription subscriber = notificationCenter.Subscribe<BroadcastMessage<SimpleContent>>(
                (message, sub) => callCount++);

            // Subscribe our notification and publish a new message
            notificationCenter.Publish(new BroadcastMessage<SimpleContent>(new SimpleContent()));

            // Act
            // Unsubscribe the notification and attempt a new publish
            subscriber.Unsubscribe();
            notificationCenter.Publish(new BroadcastMessage<SimpleContent>(new SimpleContent()));

            // Assert
            Assert.AreEqual(1, callCount, "The callbacks were not fired properly");
        }

        [TestMethod]
        [TestCategory("Runtime.Game - NotificationManager")]
        public void Handler_receives_only_its_message()
        {
            // Arrange
            // Set up the first handler
            var notificationCenter = new NotificationManager();
            notificationCenter.Subscribe<BroadcastMessage<SimpleContent>>(
                (message, sub) =>
                {
                    if (message.Content.GetType() != typeof(SimpleContent))
                    {
                        Assert.Fail();
                    }
                });

            notificationCenter.Subscribe<BroadcastMessage<string>>(
                (message, sub) =>
                {
                    if (message.Content.GetType() != typeof(string))
                    {
                        Assert.Fail();
                    }
                });

            // Act
            notificationCenter.Publish(new BroadcastMessage<string>("Test"));
            notificationCenter.Publish(new BroadcastMessage<SimpleContent>(new SimpleContent()));
        }
    }
}
