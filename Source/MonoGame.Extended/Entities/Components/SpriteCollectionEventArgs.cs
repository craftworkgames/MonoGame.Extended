using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Entities.Components
{
    public sealed class SpriteCollectionEventArgs : CollectionComponentEventArgs<Sprite>
    {
        public SpriteCollectionEventArgs(Entity entity, Sprite sprite)
            : base(entity, sprite)
        {

        }

        public bool IsAnimated => Item is AnimatedSprite;
    }
}
