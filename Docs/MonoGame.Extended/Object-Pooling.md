# Object Pooling

Object pooling is an optimization pattern. It's used to improve performance, in certain cases, by re-using objects instead of allocating memory for them on demand. In C/C++, one the things object pooling has to offer is a solution to avoid [memory fragmentation](http://stackoverflow.com/questions/3770457/what-is-memory-fragmentation). In C#, we don't have to worry about memory fragmentation thanks to [garbage collection](https://msdn.microsoft.com/en-us/library/ee787088). However, garbage collection can be still be too expensive for certain parts of real-time applications, especially on mobile devices with slower CPUs and simpler garbage collectors. [More details on object pooling here](http://gameprogrammingpatterns.com/object-pool.html).

# IMPORTANT
Always profile the game for performance problems!
Using a `Pool<T>` without first profiling for the need of one may result in a *decrease* in performance in certain cases. If you are unsure, don't use the object pooling pattern.

# Creating a Pool-able Object
All objects which can be pooled need to implement the `IPoolable` interface.
The following is a code snippet with comments demonstrating how to implement the interface.

```csharp
private class MyPoolable : IPoolable
{
    private ReturnToPoolDelegate _returnAction;

    void IPoolable.Initialize(ReturnToPoolDelegate returnAction)
    {
        // copy the instance reference of the return function so we can call it later
        _returnAction = returnAction;
    }

    public void Return()
    {
        // check if this instance has already been returned
        if (_returnAction != null)
        {
            // not yet returned, return it now
            _returnAction.Invoke(this);
            // set the delegate instance reference to null, so we don't accidentally return it again
            _returnAction = null;
        }
    }
}
```

# Using Pooled Objects

## Creating a Pool
Instantiating a `Pool<T>` is similar to any generic collection, i.e `List<T>`, but the pool does require 2 parameters for it's constructor. `T` also has to implement `IPoolable`.
```csharp
var pool = new Pool<MyPoolable>(50, index => new MyPoolable());
```
The first parameter is the _capacity_ of the pool; the maximum number of object instances the pool has reference to. The second parameter is the delegate responsible for creating each object instance.

### NOTE
Having too large of a capacity will waste memory, but having too small of a capacity will limit the number of object instances that can be pooled.

All object instances are created when the pool is instantiated.

## Getting a Pooled Object

A free pooled object instance can be requested from the pool instance.

```csharp
var myPoolable = pool.Request();
```

### NOTE
If the pool is empty, the result will be `null`.

## Returning a Object to the Pool
When the object instance is no longer needed it should be returned to the pool so it can be re-used.
```csharp
myPoolable.Return();
```
