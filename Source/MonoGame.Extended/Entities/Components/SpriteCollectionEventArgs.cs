using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Entities.Components
{
    public sealed class SpriteCollectionEventArgs : EntityEventArgs
    {
        public SpriteCollectionEventArgs(Entity entity, Sprite sprite)
            : base(entity)
        {
            Sprite = sprite;
        }

        public Sprite Sprite { get; }
        public bool IsAnimated => Sprite is AnimatedSprite;
    }
}
