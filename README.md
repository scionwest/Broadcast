Broadcast
=========

Broadcast provides a simple alternative for object communication within .NET applications using a observer pattern.

Why Broadcast over IObservable?
===============================

With Broadcast, a single object can communicate with multiple objects, without knowing that the objects exist. The  implementations within .NET such as the IObserver and IObservable pattern are mostly used with Properties. They do not provide a solid mechinism for broadcasting messages or manipulating objects during a method invocation or in the middle of an asynchronous task.

Registering for notifications
=============================

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

Posting notifications
=====================

To post a notification, that the manager will broadcast to all of the observing objects, you invoke the PostNotification method.

```
public void SomeMethod()
{
    NotificationManager.PostNotification(this, "MyNotification");
}
```

Once the notification is posted, the MyMethod method above will immediately be called. The PostNotification method will broadcast synchronously to all of the observers. You can invoke the PostNotificationAsync method if you want the manager to broadcast to all observers asynchronously.
