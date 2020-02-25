

## Input
The `MonoGame.Extended.Input` library contains input listener classes that have events you can use to subscribe to input events, instead of having to poll for input changes.  It also contains extended keyboard and mouse states.

---
## Input Listeners
There are different events that you can subscribe to in the different input listener classes. Note that this is in the `MonoGame.Extended.Input.InputListeners` library, not just `MonoGame.Extended.Input`.
```c#
// Some example usage
using MonoGame.Extended.Input.InputListeners;
private MouseListener mlistener = new MouseListener;

private void OnMouseMoved(object sender, MouseEventArgs eventArgs) {
  // What you want to happen when the mouse is moving
}

protected override void Update(GameTime gameTime)
{
  // Subscribe to the event
  mlistener.MouseMoved += OnMouseMoved;
  mlistener.Update(gameTime);
  mlistener.MouseMoved -= OnMouseMoved;
}
```

---
## Extended Keyboard and Mouse States
The library also includes some new methods and properties for the keyboard and mouse states in the `KeyboardStateExtended` and `MouseStateExtended` classes.
```c#
// Some example usage
using MonoGame.Extended.Input;
private KeyboardStateExtended kstate = KeyboardExtended.GetState();
private MouseStateExtended mstate = MouseExtended.GetState();

protected override void Update(GameTime gameTime)
{
  if (mstate.PositionChanged()) {
    // Code that happens if the position of the mouse changes
  else if (kstate.WasAnyKeyJustDown()) {
    // Code that happens if there was just any key that was down
  }
}
```

The `KeyboardStateExtended` and `MouseStateExtended` classes also contain all of the original methods and properties of the regular `KeyboardState` and `MouseState` classes.

---
## More Player Indexes
The library also adds an extended player index for up to eight players from the `ExtendedPlayerIndex` class. You can access them by typing `ExtendedPlayerIndex.One` up to `ExtendedPlayerIndex.Eight`.
