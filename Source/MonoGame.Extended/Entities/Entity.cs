using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Entities
{
    public class Entity : DrawableGameComponent
    {
        private readonly List<EntityComponent> _components = new List<EntityComponent>();
        public Entity(Game game, VisualComponent visual, TransformComponent transform) : base(game) {
            VisualComponent = visual;
            visual.SetEntity(this);
            TransformComponent = transform;
            transform.SetEntity(this);
        }
        public VisualComponent VisualComponent { get; set; }
        public TransformComponent TransformComponent { get; set; }
        public override void Update(GameTime gameTime) {
            foreach (var entityComponent in _components) {
                entityComponent.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            VisualComponent.Draw(TransformComponent.TransformMatrix);
        }


        public void AddComponent(EntityComponent component) {
            component.SetEntity(this);
            _components.Add(component);
        }
        public T GetComponent<T>(string name = null) where T : EntityComponent {
            return _components.Cast<T>().FirstOrDefault(c => c != null && (name == null || c.Name == name));
        }
    }

    public abstract class TransformComponent : EntityComponent
    {
        public abstract Matrix TransformMatrix { get; }

    }

    public abstract class CollisionComponent : EntityComponent
    {
        public abstract CollisionInfo[] CheckCollide();
    }

    public abstract class VisualComponent : EntityComponent
    {
        public abstract void Draw(Matrix transform);
    }
}