# Screen Management

## Example
The `ScreenGameComponent` manages individual `Screen` objects.  Add a new `ScreenGameComponent` to your Game's `Components`, and the screen manager will pass `Initialize` `LoadContent` `UnloadContent` `Update` and `Draw` to every registered screen.
```csharp
public Game1()
{
    // Add the screen manager to your Components.
    ScreenGameComponent screenGameComponent = new ScreenGameComponent(this);
    Components.Add(screenGameComponent);
}
```
To register your class (MyScreen) that subclasses `Screen`.  Just pass it into the `Register` function.
```csharp
public Game1()
{
    ScreenGameComponent screenGameComponent = new ScreenGameComponent(this);
    screenGameComponent.Register(new MyScreen());
    Components.Add(screenGameComponent);
}
```

## Demos
[Demo.Features/Demos/ScreensDemo.cs](https://github.com/craftworkgames/MonoGame.Extended/blob/develop/Source/Demos/Demo.Features/Demos/ScreensDemo.cs)
