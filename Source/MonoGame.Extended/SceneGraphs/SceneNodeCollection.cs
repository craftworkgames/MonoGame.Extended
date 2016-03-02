using System;
using System.Collections.ObjectModel;

namespace MonoGame.Extended.SceneGraphs
{
    public class SceneNodeCollection : Collection<SceneNode>
    {
        private readonly SceneNode _parent;

        public SceneNodeCollection(SceneNode parent)
        {
            _parent = parent;
        }

        private void RemoveParent(SceneNode item)
        {
            if (item.Parent != _parent)
                throw new InvalidOperationException($"{item} does not belong to parent {_parent}");

            item.Parent = null;
        }

        protected override void ClearItems()
        {
            foreach (var sceneNode in Items)
                RemoveParent(sceneNode);

            base.ClearItems();
        }

        protected override void InsertItem(int index, SceneNode item)
        {
            item.Parent = _parent;
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            RemoveParent(Items[index]);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, SceneNode item)
        {
            item.Parent = _parent;
            base.SetItem(index, item);
        }
    }
}