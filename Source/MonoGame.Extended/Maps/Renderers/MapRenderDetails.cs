using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Maps.Renderers
{
    public class MapRenderDetails : IEnumerable<GroupRenderDetails>
    {
        private readonly List<GroupRenderDetails> _groupDetails = new List<GroupRenderDetails>();

        public void AddGroup(GroupRenderDetails details)
        {
            _groupDetails.Add(details);
        }

        public IEnumerator<GroupRenderDetails> GetEnumerator()
        {
            return _groupDetails.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}