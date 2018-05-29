using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Platformer.Components
{
    [EntityComponent]
    public class VelocityComponent //: EntityComponent
    {
        public Vector2 Velocity { get; set; }
    }
}