using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;

namespace Platformer.Components
{
    [EntityComponent]
    public class SpriteComponent //: EntityComponent
    {
        public Sprite Sprite { get; set; }
        public Color Color { get; set; }
    }
}
