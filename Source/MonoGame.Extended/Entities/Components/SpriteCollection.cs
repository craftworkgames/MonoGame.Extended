using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MonoGame.Extended.Entities.Components
{
    public class SpriteCollection : EntityComponent, ICollection<Sprite>
    {
        private readonly List<Sprite> _sprites = new List<Sprite>();

        public event EventHandler<SpriteCollectionEventArgs> SpriteAdded;
        public event EventHandler<SpriteCollectionEventArgs> SpriteRemoved;

        public int Count => _sprites.Count;
        public bool IsReadOnly => false;

        public IEnumerable<Sprite> Sprites => _sprites;
        public IEnumerable<AnimatedSprite> AnimatedSprites => 
            (IEnumerable<AnimatedSprite>)_sprites.Where(s => s is AnimatedSprite);

        public void Add(Sprite item)
        {
            _sprites.Add(item);
            SpriteAdded?.Invoke(this, CreateEventArgs(item));
        }

        public void Clear()
        {
            foreach (var sprite in _sprites)
                SpriteRemoved?.Invoke(this, CreateEventArgs(sprite));
            _sprites.Clear();
        }

        public bool Contains(Sprite item)
        {
            return _sprites.Contains(item);
        }

        void ICollection<Sprite>.CopyTo(Sprite[] array, int arrayIndex)
        {
            foreach (var sprite in array)
                SpriteAdded?.Invoke(this, CreateEventArgs(sprite));
            _sprites.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Sprite> GetEnumerator()
        {
            return _sprites.GetEnumerator();
        }

        public bool Remove(Sprite item)
        {
            bool removed = _sprites.Remove(item);
            if (removed)
                SpriteRemoved?.Invoke(this, CreateEventArgs(item));
            return removed;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sprites.GetEnumerator();
        }

        private SpriteCollectionEventArgs CreateEventArgs(Sprite sprite)
        {
            return new SpriteCollectionEventArgs(Entity, sprite);
        }
    }
}
