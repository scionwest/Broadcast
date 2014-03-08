Broadcast
=========

Broadcast provides a simple alternative for object communication within .NET applications using a observer pattern.

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
    NotificationManager.RegisterObserver(this, "MyNotification", MyMethod);
}

void MyMethod(object sender, Dictionary<string, object> userData)
{
    // Do stuff....
}
```

Note that there are no "BeganObserving" notifications sent to any objects once registration takes place. Objects must be self aware that they have registered

Posting notifications
---------------------

To post a notification, that the manager will broadcast to all of the observing objects, you invoke the PostNotification method.

```
public void SomeMethod()
{
    NotificationManager.PostNotification(this, "MyNotification");
}
```

Once the notification is posted, the MyMethod method above will immediately be called. The PostNotification method will broadcast synchronously to all of the observers. You can invoke the PostNotificationAsync method if you want the manager to broadcast to all observers asynchronously.

You should always post a notification with the object who is responsible for the notification being passed as the sender. In most cases, it should always be `this`. It is bad practice to pass data in as sender for the observers to access and parse. Instead, you should use the user data parameter option below.

Notifications with Async
------------------------

In the event that you know that you are going to have a lot of objects registered to a specific notification, you can broadcast the notification asynchronously.

```
NotificationManager.PostNotificationAsync(this, "LargeNotification");
```

Posting notifications with parameters
-------------------------------------

If you need to provide a set of parameters with your notification post, you can do so with a dictionary. The registered object will receive them in their method delegate userData dictionary.

```

class Program
{
    void Main()
    {
        var userData = new Dictionary<string, object>
        {
            { "SomeKey", "Arbitrary data can be supplied" }
        };
        
        // Post the notification.
        NotificationManager.PostNotification(this, "SomeNotification, userData);
    }
}
```

Registering children objects
----------------------------

What if you downloaded a 3rd party framework and wanted to use Broadcast on a object contained within it? For instance, the framework might include a logger class that you want to have register with NotificationManager. Since you do not have the source code, you can not register it. How do you get around this?

Thanks to anonymous, you can easily add Broadcast support to any class, even if you do not have the source code. Simply pass the object as the sender when you register and then provide a method within the object you want to use as the Action and wrap that method within an anonymous method that satisfies the required method signature. In the example below, the Log.WriteInformation method does not contain two arguments that matches the required method delegate signature for registering a observer. So we create an anonymous method that contains two arguments that match the signature and place the WriteInformation method invocation within it.

```
public class MyObject
{
    public ExternalLogger Log {get;set;}
    
    public MyObject()
    {
        NotificationManager.RegisterObserver(Log, "LogInformation", (x, y) => Log.WriteInformation());
        NotificationManager.RegisterObserver(Log, "LogError", (x, y) => Log.WriteError());
    }
}
```

With the above example, every time you Post a LogInformation or LogError notification, your 3rd party log class will execute the appropriate methods.

Note that if you want to unregister from the notification manager, you must pass Log as the sender.

Using this technique, you can abstract your application away from relying on a 3rd party framework in a very tightly bound manor. Using this abstracted approach will let you swap out the logger class with a custom one in the future without having to change "Log.WriteInformation" all over in your source. You just need to change it in one location, the property declaration and the registration to NotificationManager.

Unregistering a single object from notifications
------------------------------------------------

Unregistering a single observer from notifications is simple. You just invoke the UnregisterObserver method.

```
NotificationManager.UnregisterObserver(this, "MyNotification");
```

There might be times where manually unregistering an observer is needed.

Unregistering all objects associated with a specific notification
-----------------------------------------------------------------

Unregistering all observers that share the same notification is just as simple as removing a single object.

```
NotificationManager.UnregisterAllObservers("MyNotification");
```

Unregistering all observers and all notifications.
--------------------------------------------------

In order to unregister every observer that has previously registered to receive notifications uses the UnregisterAllObservers method. You can use this to clear out all notification registerations in one shot.

```
NotificationManager.UnregisterAllObservers();
```

Note that there is no notification provided to observers that they are being unregistered. 

Unregistering null objects
--------------------------

The NotificationManager is self cleaning. Meaning that as objects become null, it will handle unregistering them on its own. You don't have to worry about notifications trying to post to null objects, as all null objects are purged from its internal collection after each Post is performed.

Posting to the Main UI Thread
-----------------------------

While you are working with UI controls, you can only access them while on the main thread. It is possible that another object has posted a notification from a different thread and your UI controls receive the notification and react to it. What happens then is that any attempt to access a UI control will throw an exception. 

In order to work around this, I provided the ability to attach a SynchronizationContext to the manager. This provides the manager with asynchronous support and not require the UI code-behind classes to perform a ton of main thread checks. You grab the main thread `SynchronizationContext` from within your MainWindow (WPF) or any Form (WinForms) constructor and assign it to the `NotificationManager.Context`property.

```
    NotificationManager.Context = System.Threading.SynchronizationContext.Current;
```

By setting the `Context` property, the NotificationManager will post all Synchronous broadcasts to that thread. If that property is null, the NotificationManager will post all broadcasts to the calling thread. Note that it is possible to perform a synchronous broadcast from another thread other than the main. The caller might be running asynchronous, even though a synchronous post was invoked. That is why assigning the SynchronizationContext is important. It ensures that synchronous posts are guaranteed to be ran on the main thread.

Forcing a Synchronous broadcast from an asynchronous post
-------------------------------------------------------

If an asynchronous post is broadcasted, but you want to force an object to handle the broadcast in a synchronous fashion (such as a UI element), then simply providing the `Context` is not enough. Setting the `Context` ensures that synchronous posts are ran on the main thread, but do not guarantee that an async post will run on the main thread. 

In order to force an object to run on the main thread, even if an async post was broadcasted, you can register the observer with an overloaded registration method. A 3rd parameter (boolean) can be used to inform the NotificationManager that this particular observer can never run async and must always run synchronous. If the `Context` is not null then it forces this observer to run on the main thread.

```
    NotificationManager.RegisterObserver(this, "SomeNotification", this.HandleNotification, canRunAsync: false);

```
