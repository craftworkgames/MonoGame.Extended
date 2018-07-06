# Entity-Component-System

MonoGame.Extended supports an Entity Component System (ECS) which allows for the use of an entity that contains components and systems (controllers for the components). You can see a finished demo [here](https://github.com/craftworkgames/MonoGame.Extended/tree/develop/Source/Demos/Demo.Platformer) and read more about ECS's [here](https://en.wikipedia.org/wiki/Entity%E2%80%93component%E2%80%93system).

### Getting Started

First create the `EntityComponentSystem` in your main game class and store the `EntityManger` it will be useful later on as its used alot.

```csharp
public GameMain() {
  ...
  _ecs = new EntityComponentManager(this);
     _entityManager = _ecs.EntityManager;

  _ecs.Scan(Assembly.GetExecutingAssembly());
  ...
}
```

Next call the `Update()` and `Draw()`in their respective methods of the main game class.

```csharp
public Update(GameTime gameTime) {
  ...
  _ecs.Update(gameTime);
}

public Draw(GameTime gameTime) {
  ...
  _ecs.Draw(gameTime);
}
```

Now you're ready to roll with the Entity-Component-System.

### Entity Factories/Templates

You will need to create entity factories that handle the creation of an entity with its attached components. This can be accomplished through factories or templates.

###### Entity Factory Example

```csharp
public class EntityFactory {
  private readonly EntityComponentSystem _ecs;
  private readonly EntityManager _entityManager;

  private Texture2D playerTexture;

  public EntityFactory(EntityComponentSytem ecs, EntityManager entityManager) {
    _ecs = ecs;
    _entityManage = entityManager;
  }

  // Load in con
  public void LoadContent(ContentManager content) {
    // Load the player sprite and store it for later use.
    playerTexture = content.Load<Texture2D>("Sprites/Player");
  }

  // Create a player entity with a transform and sprite.
  public Entity CreatePlayer(Vector2 position) {
    // Create the entity. This adds the enitty to the game as well.
    var entity = _entityManager.CreateEntity();

    // Attach a transform to the entity.
    var transform = entity.Attach<TransformComponent2D>();
    transform.Position = position;

    // Attach a sprite to the entity.
    var sprite = entity.Attach<SpriteComponent>();
    sprite.Texture = playerTexture;
    sprite.Origin = new Vector2(playerTexture.Width / 2f, playerTexture.Height / 2f);

    return entity;
  }
}
```

###### Entity Template Example

```csharp
[EntityTemplate(Name)]
public class PlayerTemplate : EntityTemplate {
    public const string Name = "PlayerTemplate";

  // Attach components and set properties of entity.
  protected override void Build(Entity entity) {
      entity.Group = "PLAYER";

    entity.Attach<TransformComponent2D>();

    var sprite = entity.Attach<SpriteComponent>();
    sprite.Texture = _playerTexture;
  }
}
```

*Note that the components used in these example were created before use and contain their own component and system. Examples of components and systems are documented later in this document.*

### Entity Components

An entity component acts as a container for properties to be added to an entity and to later be used by systems or other entities. Components can also be pooled using the `EntityComponentPool` which can be read about more [here](http://craftworkgames.github.io/MonoGame.Extended/MonoGame.Extended/Object-Pooling/). Creating a component is simple.

```csharp
[EntityComponent]
[EntityComponentPool(InitialSize = 100)] // This will create a pool of 100 EnemyComponents.
public class EnemyComponent : EntityComponent {
    public float Health { get; set; }
  public float Speed { get; set; }
  public int Strength { get; set; }

  // Reset the values of the component for reuse of the component.
  public override void Reset() {
    Health = 100;
    Speed = 1;
    Strength = 1;
  }
}
```

### Entity Systems

An entity system acts as a controller for components in an entity, it contains a method called `Process()` which is ran on either the update cycle or the draw cycle but only on one, it is up to you to determine which one the system should use through the `EntitySystem` attributes `GameLoopType`paramter. For example, you would use the draw cylce when drawing items with the system and you would use the update cylce for handling tasks like user input processing and other general tasks. To determine when a system should be used there is an attribute called `Aspect` that selects a set of components that the entity must contain for the system to become active in the entity.

```csharp
[Aspect(AspectType.All, typeof(PlayerComponent), typeof(TransformComponent2D))]
[EntitySystem(GameLoopType.Update, Layer = 0)]
public class MovementSystem : EntityProcessingSystem {
	private KeyboardState _keyboardState;
  
	protected override void Begin(GameTime gameTime) {
  	base.Begin(gameTime);
    
    _keyboardState = Keyboard.GetState();
  }
  
  protected override void Process(GameTime gameTime, Entity entity) {
  	var transform = entity.Get<TransformComponent2D>();
    var player = entity.Get<PlayerComponent>();
    
    Vector2 movementDirection = Vector2.Zero;
    
    if (_keyboardState.IsKeyDown(Keys.Left)) {
    	movementDirection.X -= 1;
    }
    if (_keyboardState.IsKeyDown(Keys.Right)) {
    	movementDirection.X += 1;
    }
    if (_keyboardState.IsKeyDown(Keys.Up)) {
    	movementDirection.Y -= 1;
    }
    if (_keyboardState.IsKeyDown(Keys.Down)) {
    	movementDirection.Y += 1;
    }
    
    movementDirection.Normalize();
    
    transform.Postion += movementDirection * player.Speed * gameTime.ElapsedGameTime.TotalSeconds;
  }
}

```
