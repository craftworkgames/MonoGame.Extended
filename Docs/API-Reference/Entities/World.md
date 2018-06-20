The `EntityWorld` is the entry point to the ECS. It holds your entities and systems and you'll use it later to create and destroy entities.

## Creating the world

Usually you'll create the world as a field of your main `Game` class or some similar entry point. Then you register your systems.

```csharp
_world = new EntityWorld();
_world.RegisterSystem(new PlayerSystem());
_world.RegisterSystem(new RenderSystem(GraphicsDevice));
```

## Updating the world

Once the world is created you'll also need to call `Update` and `Draw` in your games `Update` and `Draw` methods respectively.

```csharp
protected override void Update(GameTime gameTime)
{
    _world.Update(gameTime);
    base.Update(gameTime);
}
```

```csharp
protected override void Draw(GameTime gameTime)
{
    _world.Draw(gameTime);
    base.Draw(gameTime);
}
```
