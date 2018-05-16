<a alight="right" href="https://www.mapeditor.org/"><img align="right" src="https://raw.githubusercontent.com/bjorn/tiled/master/src/tiled/images/about-tiled-logo.png" height="200"></a>

# Tiled
Tiled is an [open sourced](https://github.com/bjorn/tiled) free to use "generic tile map editor". Tiled lets you easily design and view tile maps, and through the `Monogame.Extended.Tiled` package, you can load and display a map generated with Tiled in monogame

To load a TiledMap into your project, you first need to add it to your `ContentManager`
In order to compile the tile map for use in `MonoGame` you must add a reference to the pipeline tool
First open the `Content.mgcb` file in the `Content/` folder

![Imgur](https://i.imgur.com/fvHuZcu.png)

Add a reference to `MonoGame.Extended.Content.Pipeline.dll` in the `packages/MonoGame.Extended.Content.Pipeline.X.X.X/tools/` directory

Once the reference is added, you can add the font file and texture to the content. If all goes well, the importer and processor should be selected automatically.

Don't forget to **Rebuild** your content.

# Using the map in your game

To create and render a map you will need two new properties
```c#
// The tile map
private TiledMap map;
// The renderer for the map
private TiledMapRenderer mapRenderer;
```

In your `Initialize()` method, you can initialize the two methods
```c#
protected override void Initialize() {
    base.Initialize();

    // Load the compiled map
    map = Content.Load<TiledMap>("path/to/your/map/file");
    // Create the map renderer
    mapRenderer = new TiledMapRenderer(GraphicsDevice);
}
```

To finally render and tick the map, call `mapRenderer.Update();` and `mapRenderer.Draw();` in their respective methods
```c#
protected override void Update(GameTime gameTime) {
    // Update the map
    // map Should be the `TiledMap`
    mapRenderer.Update(map, gameTime);

    base.Update(gameTime);
}

protected override void Draw(GameTime gameTime) {
    // Clear the screen
    GraphicsDevice.Clear(Color.Pink);
    
    // Transform matrix is only needed if you have a Camera2D
    // Setting the sampler state to `SamplerState.PointClamp` is reccomended to remove gaps between the tiles when rendering
    spriteBatch.Begin(transformMatrix: camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

    // map Should be the `TiledMap`
    // Once again, the transform matrix is only needed if you have a Camera2D
    mapRenderer.Draw(map, camera.GetViewMatrix());
    
    // End the sprite batch
    spriteBatch.End();

    base.Draw(gameTime);
}
```

Good luck and happy coding! :)
