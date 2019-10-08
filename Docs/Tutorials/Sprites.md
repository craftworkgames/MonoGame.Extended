# Sprites

A sprite in computer graphics terms most often refers to an image drawn to the screen in a 2D game. A sprite can be a single image, but quite often you'll want sprites to be animated.

## Drawing Basic Sprites

![apple](images/sprites/apple.png)

<small>source: [gamedevmarket.net](https://www.gamedevmarket.net/asset/icons-food-9999/?ally=yJRl98tX)</small>

Drawing a basic sprite with MonoGame can be done using the `SpriteBatch`. First, load a `Texture2D` with the `ContentManager` in the `LoadContent` method.

```cs
protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _apple = Content.Load<Texture2D>("apple");
}
```

Then, in the `Draw` method you can draw the texture between the `Begin` and `End` calls of the `SpriteBatch`.

```cs
protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);

    _spriteBatch.Begin();
    _spriteBatch.Draw(_apple, new Vector2(100, 100), Color.White);
    _spriteBatch.End();

    base.Draw(gameTime);
}

```

Drawing sprites directly from the `Texture2D` can be useful in certain scenarios, like drawing backgrounds, but it can be quite limiting when you want to do animations.
