namespace MonoGame.Extended.Entities.Components
{
    public class CollectionComponentEventArgs<T> : EntityEventArgs
    {
        public CollectionComponentEventArgs(Entity entity, T item)
            : base(entity)
        {
            Item = item;
        }

        public T Item { get; }
    }
}
