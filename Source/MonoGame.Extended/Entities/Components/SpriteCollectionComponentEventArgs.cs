using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Entities.Components
{
    public sealed class SpriteCollectionComponentEventArgs : CollectionComponentEventArgs<Sprite>
    {
        public SpriteCollectionComponentEventArgs(Entity entity, Sprite sprite)
            : base(entity, sprite)
        {

        }

        public bool IsAnimated => Item is AnimatedSprite;
    }
}
