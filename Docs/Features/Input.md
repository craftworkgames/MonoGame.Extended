
## Input
The `MonoGame.Extended.Input` library contains input listener classes that have events you can use to subscribe to input events, instead of having to poll for input changes. 

First, add the library to your code like this:
```c#
using MonoGame.Extended.Input;
```

---
## Extended Keyboard and Mouse States
The library also includes new methods for the keyboard and mouse states in the `KeyboardStateExtended` and `MouseStateExtended` classes.
```c#
// Some example usage
KeyboardStateExtended kstate = KeyboardExtended.GetState();
MouseStateExtended mstate = MouseExtended.GetState();

protected override void Update(GameTime gameTime)
{
  if (mstate.PositionChanged()) {
    // Code that happens if the position of the mouse changes
  else if (kstate.WasAnyKeyJustDown()) {
    // Code that happens if there was just any key that was down
  }
}
```
**List of New Keyboard Methods**
| Method | Description |
| --- | --- |
| `bool IsShiftDown()` | returns if any of the 2 shift keys are down |
| `bool IsControlDown()` | returns if any of the 2 ctrl keys are down |
| `bool IsAltDown()` | returns if any of the 2 alt keys are down |
| `bool WasKeyJustDown(Keys key)` | returns if a key was just down but now is up |
| `bool WasKeyJustUp(Keys key)` | returns if a key was just up but now is down |
| `bool WasAnyKeyJustDown()` | returns if any keys were just down |

**List of New Mouse Methods/Properties**
| Method | Description |
| --- | --- |
| `bool PositionChanged` | returns if the mouse's position changed since the last check |
| `int DeltaX` | returns the change in the X-coordinate of the mouse |
| `int DeltaY` | returns the change in the Y-coordinate of the mouse |
| `Point DeltaPosition` | returns `new Point(DeltaX, DeltaY)` |
| `int DeltaScrollWheelValue` | returns the change in the scroll wheel value of the mouse |
| `bool IsButtonDown(MouseButton button)` | returns if a mouse button is currently pressed |
| `bool IsButtonUp(MouseButton button)` | returns if a mouse button is currently released |
| `bool WasButtonJustDown(MouseButton button)` | returns if a mouse button was just pressed but now is released |
| `bool WasButtonJustUp(MouseButton button)` | returns if a mouse button was just released but now is pressed |

The `KeyboardStateExtended` and `MouseStateExtended` classes also contain all of the original methods and properties of the regular `KeyboardState` and `MouseState` classes.

---
## More Player Indexes
The library also adds an extended player index for up to eight players from the `ExtendedPlayerIndex` class. You can access them by typing `ExtendedPlayerIndex.One` up to `ExtendedPlayerIndex.Eight`.
