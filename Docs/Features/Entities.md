# Entities

The Entities package is a modern high performance Artemis based Entity Component System. Many of the features found in this implementation were inspired by [artemis-odb](https://github.com/junkdog/artemis-odb/). Although, many others were also studied during development. As you'll see the systems are designed to feel familar to MonoGame developers.

## What is an ECS?

An [Entity Component System (ECS)](https://www.gamedev.net/articles/programming/general-and-gameplay-programming/understanding-component-entity-systems-r3013) is a way to build and manage the entities (or game objects) in your game by composing their component parts together. An ECS consists of three main parts:

### Components

A component is simply a class that holds some state about the entity. Typically, components are lightweight and don't contain any game logic. It's common to have components with only a few properties or fields. Components can be more complex but inheritence is not encouraged.


### Entities

An entity is a composition of components identified by an ID. Often you only need the ID of the entity to work with it. For performance reasons, and entity ID is only valid while the entity is alive. Once the entity is destroyed, it's ID may be recycled.

### Systems

A system is a class that will run during the game's `Update` or `Draw` calls. They usually contain the game logic about how to manage a filtered collection of entities and their components. 

## Creating the world

The `World` is the entry point to the ECS. It holds your entities and systems and you'll use it later to create and destroy entities.

To create the world you need to use the `WorldBuilder` and add your systems before building the `World` instance.

```csharp
_world = new WorldBuilder()
    .AddSystem(new PlayerSystem())
    .AddSystem(new RenderSystem(GraphicsDevice))
    .Build();
```

*Note:* Manually adding your systems this way might seem annoying at first, but it can be highly desireable to be able to control the order systems are added. It also allows you to constructor inject services as desired.

Once the world is created you need to call the `Update` and `Draw` methods. 

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

*Note:* The world also implements the `IGameComponent` interface, so if you prefer you can add it to the `GameComponentCollection` instead (not to be confused with ECS components).

## Creating entities

Usually when you create an entity you'll want to attach some components to it immediately. This is not required though, as components can be added and removed anytime by systems.

```csharp
var entity = _world.CreateEntity();
entity.Attach(new Transform2(position));
entity.Attach(new Sprite(textureRegion));
```

Any standard class can be used as a component but typically you'll want to keep your components lightweight and specific.

*Note:* An entity can only have one instance of each component type.

## Destroying entities

Removing entities from the world is easy.

```csharp
_world.DestroyEntity(entity);
```

It should be noted that the actual entity creation and removal is deferred until the next update. This allows for some performance optimizations and batches events so that they can be handled more gracefully by systems.

*Note:* When you're inside an `EntitySystem` there are helper methods for creating destroying entities so that you don't need to access the `World` instance each time.

## Types of systems

Systems can be used to do all kinds of processing during your game. There are several kinds of base systems available to build your game.

- An `UpdateSystem` is a basic system that has an `Update` method called every frame.
- A `DrawSystem` is a basic system that has a `Draw` method called every frame.
- An `EntityUpdateSystem` is used to process a filtered collection of entities during the `Update` call.
- An `EntityDrawSystem` is used to process a filtered collection of entities during the `Draw` call.
- An `EntityProcessingSystem` can be used to process a filtered collection of entities one at a time during the `Update` call. 
- You can also create a system that has both an `Update` method and a `Draw` method by implementing the `IUpdateSystem` and `IDrawSystem` interfaces respectively.
- An `EntitySystem` is the base class for all entity processing systems. Typically you won't derive from this class unless you're building a new type of entity processing system. If you do derive from this class you probably also want to implement one of the update or draw interfaces.

## Creating systems

To create a new system, decide which base system to derive from and implement a new class.

```csharp
public class RenderSystem : EntityDrawSystem
```

When you're creating entity systems the first thing you'll want to do is provide an `Aspect` to filter the system to only process the entities you're interested in.

For example, a typical `RenderSystem` might want to process entities with a `Sprite` component and a `Transform2` component. To provide an aspect you pass it into the base constructor.

```csharp
public RenderSystem(GraphicsDevice graphicsDevice)
    : base(Aspect.All(typeof(Sprite), typeof(Transform2)))
{
    _graphicsDevice = graphicsDevice;
    _spriteBatch = new SpriteBatch(graphicsDevice);
}
```

Next, you'll need to override the `Update` or `Draw` method (depending on what type of system you're implementing).

In our case, the `Draw` method might look something like this:

```csharp
public override void Draw(GameTime gameTime)
{
    _spriteBatch.Begin();

    foreach (var entity in ActiveEntities)
    {
      // draw your entities
    }

    _spriteBatch.End();
}
```

*Note:* Don't forget to add your system to the `WorldBuilder` when you're done.

## Accessing components

The preferred way to access components is to use component mappers.

A `ComponentMapper` provides a very fast way to access components within a system. When you're using a component mapper you're getting nearly direct access to the underlying arrays that hold the components under the hood.

To get a component mapper, create a field on your system and use the `Initialize` method to grab an instance of the mapper. Do this for each component type you want to process.

```csharp
public override void Initialize(IComponentMapperService mapperService)
{
  _transformMapper = mapperService.GetMapper<Transform2>();
  _spriteMapper = mapperService.GetMapper<Sprite>();
}
```

Then inside the `Update` or `Draw` method you can get access to the components for each entity you want to process.

```csharp
var transform = _transformMapper.Get(entityId);
var sprite = _spriteMapper.Get(entityId);

_spriteBatch.Draw(sprite, transform);
```

Component mappers can also be used to modify entities on the fly. For example, you can add a new component to an entity with the `Put` method.

```csharp
_buffMapper.Put(entityId, buffComponent);
``` 

*Note:* The `Put` method will replace an existing component of the same type if it already exists. There is no need to check if the entity already has the component.

You can also check if an entity `Has` a component or `Delete` a component with the mapper.

---

For convienience it's also possible to access components on an entity *without* using component mappers. This can be useful for prototyping ideas or when performance isn't a primary concern.

```csharp
var entity = GetEntity(entityId);
var health = entity.Get<HealthComponent>();
var transform = entity.Get<Transform2>();
```

*Note:* This method of accessing components requires dictionary lookups of the component types each frame. This is still a fairly fast operation, and for some games it'll do just fine.

## Filtering components

An `Aspect` is used by entity systems to decide what component types the system will process. The entities will be available in the system's `ActiveEntities` collection.

An aspect has three methods:

- `Aspect.All(A, B)` requires the entities to have all of the desired components.
- `Aspect.One(C, D)` requires the entities to have any one or more of the components.
- `Aspect.Exclude(E, F)` will exclude entities that have any of these components.

Aspects can also be chained together. For example, an entity matching:

`Aspect.All(A, B).One(C, D).Exclude(E)` would need to have A and B and at least one of C or D except if it has E.
