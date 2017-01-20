using System.Collections.Generic;

namespace MonoGame.Extended.Entities.Components
{
    public class GenericCollectionComponent<T>
        : CollectionComponent<T, CollectionComponentEventArgs<T>>
    {
        new public IReadOnlyCollection<T> Collection => base.Collection;

        protected override CollectionComponentEventArgs<T> CreateEventArgs(T item)
        {
            return new CollectionComponentEventArgs<T>(Entity, item);
        }
    }
}
