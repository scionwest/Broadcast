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

Unregistering from notifications
================================

Unregistering a observer from notifications is simple. You just invoke the UnRegisterObserver method.
```
NotificationManager.UnRegisterObserver(this, "MyNotification");
```

Posting notifications with parameters
=====================================

If you need to provide a set of parameters with your notification post, you can do so with a dictionary. The registered object will receive them in their method delegate userData dictionary.

```
class Person
{
    string Name {get;set;}
    Person SignificantOther {get;set;}
    List<Person> Children {get;set;}
    
    Person()
    {
        // Register for the notification.
        NotificationManager.RegisterObserver(this, "ChildBorn", AddChild);
    }
    
    // Receives the notification.
    void AddChild(object sender, Dictionary<string, object> userData)
    {
        var child = new Person();
        
        if (userData.ContainsKey("Name"))
            child.Name = userData["Name"] as string;
            
        this.Children.Add(child);
        
        if (this.SignificantOther == null && sender is Person)
            this.SignificantOther = sender as Person; // Assumed to be the mom of child; therefore significant other.
    }
}

class Program
{
    void Main()
    {
        var mother = new Person();
        mother.Name = "Alice";
        
        // Create our parameter - Not a good example; you will get the idea though.
        var childName = new Dictionary<string, object>();
        childName.Add("Name", "Joey");
        
        // Post the notification.
        NotificationManager.PostNotification(mother, childName);
    }
}
```

Notifications with Async
========================

In the event that you know that you are going to have a lot of objects registered to a specific notification, you can broadcast the notification asynchronously.

```
NotificationManager.PostNotificationAsync(this, "LargeNotification");
```
