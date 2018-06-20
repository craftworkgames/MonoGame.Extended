Entities are containers for components. By itself and entity is just an ID that is used to lookup the components when it is being processed by systems.

## Creating entities

Usually when you create an entity you'll want to attach some components to it immediately. This is not required though, as components can be added and removed anytime.

```csharp
var entity = _world.CreateEntity();
entity.Attach(new Transform2(position));
entity.Attach(new Sprite(textureRegion));
```

Any standard class can be used as a component but typically you'll want to keep your components light and specific.

## Destroying entities

Removing entities from the world is easy.

```csharp
_world.DestroyEntity(entity);
```

It should be noted that the actual entity creation and removal is deferred until the next update. This allows for some performance optimizations and batches events so that they can be handled more gracefully by systems.
