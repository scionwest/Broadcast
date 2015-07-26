Broadcast Notifications
=========

_Current Version: 2.0_


Broadcast Notifications provides a simple alternative for object communication within .NET applications using a PubSub pattern.

Why Broadcast over .NET's IObserver?
------------------------------

With Broadcast, a single object can communicate with multiple objects in a non-generic manor, without knowing that the objects exist. The  implementations within .NET such as the IObserver and IObservable pattern are a very cumbersome subscription model for method invocation. The solution .NET provides is not very elegant and forces your application to be tightly bound to the objects. Broadcast aims to decouple objects, providing an easier path for developers to make their application more maintainable.

The .NET implementation uses Generics, so you must create a Observable repository for each Type you wish to Observe. With Broadcast's NotificationManager, you can mix and match registered Types with the manager. This allows you to focus on handling notifications rather than writing custom observer objects for every Observable Type in your project.

The .NET implementation requires all Type's that want to subscribe to the Observer and react to notifications, to implement a lot more code. They require you to strongly type your observer to the same Type that your Observable is Typed to. So each class can only observe one object. On top of that, you can not have any class pass a non-observer Type to the Observer. Broadcast can as demonstrated below under the Registering Children Observers.

Note: The NotificationManager is thread-safe. You can call each of these and not worry about races, nor worry about unregistering or registering objects while a post is taking place on another thread.

Registering for notifications
-----------------------------

To register for your object to receive notifications, you create a method that you want to have the manager target when a notification happens, then just invoke the RegisterObserver static method.

```
// Constructor for MyObject
public MyObject()
{
    notificationCenter.Subscribe<BroadcastMessage<string>>(
        (message, sub) =>
        {
            Console.WriteLine(message.Content);
        });
}
```

You can also subscribe with a conditional lambda that must pass in order for the subscriber to receive messages.

```
// Constructor for MyObject
public MyObject()
{
    notificationCenter.Subscribe<BroadcastMessage<string>>(
        (message, sub) =>
        {
            Console.WriteLine(message.Content);
        },
        (message) => message.Content.Equals("Opening"));
}
```

In the above example, the `Console.WriteLine` method will only be invoked if the content of the message is equal to `Opening`.

Posting notifications
---------------------

To post a notification, that the manager will broadcast to all of the observing objects, you invoke the PostNotification method.

```
public void SomeMethod()
{
    notificationCenter.Publish(new BroadcastMessage<string>("Test"));
}
```

Once the notification is posted, the subscription callback above will immediately be called. The PostNotification method will broadcast synchronously to all of the observers. You can invoke the PostNotificationAsync method if you want the manager to broadcast to all observers asynchronously.

Unsubscribing
-------------

The `INotificationCenter.Subscribe` method returns an instance of `ISubscription` which can be used to unsubscribe from publications. You can either unsubscribe via the returned `ISubscription` instance, or you can unsubscribe using the provided `ISubscription` instance in the subscription callback.

    // Subscribe our notification and publish a new message
    notificationCenter.Publish(new BroadcastMessage<SimpleContent>(new SimpleContent()));

    ISubscription subscriber = notificationCenter.Subscribe<BroadcastMessage<SimpleContent>>(
        (message, sub) => 
        {
            // Unsubscribe from within the callback based on the result.
            if (message.Content == "Closed")
                sub.Unsubscribe();
        });

    // Unsubscribe the notification and attempt a new publish
    subscriber.Unsubscribe();
    
Message Content
---------------

You may send in any class that implements the `IMessage<T>` interface as a publication. The API includes a `BroadcastMessage<T>` class that can be used to wrap simple messages, like a string, or a class.

    notificationCenter.Publish(new BroadcastMessage<SimpleContent>(new SimpleContent("Content Generated")));
    
You may opt in to creating as many classes as you want that implement `IMessage<T>`, allowing for your messages to contain complex logic.