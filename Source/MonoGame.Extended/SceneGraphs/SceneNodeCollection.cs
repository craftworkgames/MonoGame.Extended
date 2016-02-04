using System.Collections.ObjectModel;

namespace MonoGame.Extended.SceneGraphs
{
    public class SceneNodeCollection : Collection<SceneNode>
    {
        public SceneNodeCollection(SceneNode parentNode)
        {
            _parentNode = parentNode;
        }

        private SceneNode _parentNode;
    }
}