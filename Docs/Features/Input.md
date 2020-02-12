
## Input
The `MonoGame.Extended.Input` library contains input listener classes that have events you can use to subscribe to input events, instead of having to poll for input changes. 

First, add the library to your code like this:
```c#
using MonoGame.Extended.Input;
```

## Keyboard State
You can access the extended keyboard state like this:
```c#
KeyboardStateExtended kstate = KeyboardExtended.GetState();
```

## More Player Indexes
The `MonoGame.Extended.Input` library also adds an extended player index for up to eight players from the `ExtendedPlayerIndex` class. You can do `ExtendedPlayerIndex.One` up to `ExtendedPlayerIndex.Eight`.
