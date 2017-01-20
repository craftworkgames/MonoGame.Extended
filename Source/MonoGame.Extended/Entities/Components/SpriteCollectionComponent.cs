using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonoGame.Extended.Entities.Components
{
    public class SpriteCollectionComponent : CollectionComponent<Sprite, SpriteCollectionComponentEventArgs>
    {
        private List<AnimatedSprite> _animatedSprite = new List<AnimatedSprite>();

        public IReadOnlyCollection<Sprite> Sprites => Collection;
        public IReadOnlyCollection<AnimatedSprite> AnimatedSprites => new ReadOnlyCollection<AnimatedSprite>(_animatedSprite);

        public SpriteCollectionComponent()
        {
            ItemAdded += SpriteCollection_ItemAdded;
            ItemRemoved += SpriteCollection_ItemRemoved;
        }

        protected override SpriteCollectionComponentEventArgs CreateEventArgs(Sprite sprite)
        {
            return new SpriteCollectionComponentEventArgs(Entity, sprite);
        }

        private void SpriteCollection_ItemAdded(object sender, SpriteCollectionComponentEventArgs e)
        {
            ModifyCollection(e.Item, _animatedSprite.Add);
        }

        private void SpriteCollection_ItemRemoved(object sender, SpriteCollectionComponentEventArgs e)
        {
            ModifyCollection(e.Item, i => _animatedSprite.Remove(i));
        }

        private void ModifyCollection(Sprite sprite, Action<AnimatedSprite> action)
        {
            var animatedSprite = sprite as AnimatedSprite;
            if (animatedSprite != null)
                action(animatedSprite);
        }
    }
}
