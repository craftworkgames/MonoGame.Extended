using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonoGame.Extended.Entities.Components
{
    public class SpriteCollection : CollectionComponent<Sprite, SpriteCollectionEventArgs>
    {
        private List<AnimatedSprite> _animatedSprite = new List<AnimatedSprite>();

        public IReadOnlyCollection<Sprite> Sprites => Collection;
        public IReadOnlyCollection<AnimatedSprite> AnimatedSprites => new ReadOnlyCollection<AnimatedSprite>(_animatedSprite);

        public SpriteCollection()
        {
            ItemAdded += SpriteCollection_ItemAdded;
            ItemRemoved += SpriteCollection_ItemRemoved;
        }

        protected override SpriteCollectionEventArgs CreateEventArgs(Sprite sprite)
        {
            return new SpriteCollectionEventArgs(Entity, sprite);
        }

        private void SpriteCollection_ItemAdded(object sender, SpriteCollectionEventArgs e)
        {
            ModifyCollection(e.Item, _animatedSprite.Add);
        }

        private void SpriteCollection_ItemRemoved(object sender, SpriteCollectionEventArgs e)
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
