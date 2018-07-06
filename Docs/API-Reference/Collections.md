# Collections

Collections are extensions to the C# collections that are useful for game programming.

## Bag

A `Bag` is an un-ordered array of items with fast Add and Remove properties.

It is much faster than an array when removing items, and takes less space than a linked list.

Bag will resize itself only when it needs to.

```csharp
var bag = new Bag<int>(3);
bag.Add(4);
bag.Add(8);
bag.Add(15);
// bag is now [4, 8, 15]

bag.Add(16); // array is extended here, capacity is now 4 instead of 3

bag.Remove(1);
// bag is now [4, 16, 15] with a capacity of 4
```

## BitVector32

The `BitVector32` class provides easy access to 32 individual bit flags.

## Deque

Represents a collection of objects which elements can added to or removed either from the front or back; a
[double ended queue](https://en.wikipedia.org/wiki/Double-ended_queue).

## DictionaryExtensions

Extends all `Dictionary<>` classes with `GetValueOrDefault(key, default)`.

## KeyedCollection

A KeyedCollection is used like a `Dictionary<>`, except you provide a function that takes a value and return's it's key.

## ListExtensions

Adds `Shuffle(Random)` to all `IList<>` classes.

## [Object Pooling](Object-Pooling.md)

An `ObjectPool<T>` allows reuse of memory for a group of items to avoid Garbage Collection.
More information is in the [Object Pooling](Object-Pooling.md) documentation.

## ObservableCollection

An `ObservableCollection<T>` manages an `IList<T>` of items firing `ItemAdded`, `ItemRemoved`, `Clearing` and `Cleared` events when the collection is changed.

