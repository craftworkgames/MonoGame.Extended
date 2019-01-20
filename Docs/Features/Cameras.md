# Cameras

## Orthographic Camera

The purpose of the camera is to create a transformation matrix that changes the way a sprite batch is rendered.

To create a camera initialize an instance of it using one of the constructor overloads. It's recommended that you used a viewport adapter to scale the screen but you don't have to.
```csharp
        private Camera2D _camera;

        protected override void Initialize()
        {
            base.Initialize();

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);
        }
```
Next you'll need to apply the camera's view matrix to one or more of the `SpriteBatch.Begin` calls in your `Draw` method.
```csharp
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // the camera produces a view matrix that can be applied to any sprite batch
            var transformMatrix = _camera.GetViewMatrix();
            _spriteBatch.Begin(transformMatrix: transformMatrix);
            // ... draw sprites here ...
            _spriteBatch.End();
        }
```
A [transformation matrix](https://msdn.microsoft.com/en-us/library/ff433701.aspx) is one of the parameters of a `SpriteBatch.Begin` call.

> Transformation matrix for scale, rotate, translate options.

In other words, we use the camera to transform the way a batch of sprites is rendered to the screen without actually modifying their positions, rotations or scales directly. This creates the effect of having a camera looking at your scene that can move, rotate and zoom in and out.

Once you've got a camera instance in your game you'll probably want to move it around in the `Update` method somehow. For example, you could move the camera's position with the arrow keys.
<a name="moveUsage"></a>
```csharp
        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            const float movementSpeed = 200;

            if (keyboardState.IsKeyDown(Keys.Up))
                _camera.Move(new Vector2(0, -movementSpeed) * deltaTime);
        }
```
Last but not least, there'll be times when you want to convert from screen coordinates to world coordinates and visa-vera.  For example, if you want to know which sprite is under the mouse you'll need to convert the mouse position back into the world position that was used to position the sprite in the first place.
```csharp
    var mouseState = Mouse.GetState();
    _worldPosition = _camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y));
```
# Further Reading

[Transformation Matrix on MSDN](https://msdn.microsoft.com/en-us/library/ff433701.aspx)
[Matrix Basics](https://stevehazen.wordpress.com/2010/02/15/matrix-basics-how-to-step-away-from-storing-an-orientation-as-3-angles/)
