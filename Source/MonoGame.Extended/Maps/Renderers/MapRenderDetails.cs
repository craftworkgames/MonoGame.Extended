using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Maps.Renderers
{
    public class MapRenderDetails : IEnumerable<GroupRenderDetails>
    {
        private readonly List<GroupRenderDetails> _groupDetails = new List<GroupRenderDetails>();

        public IEnumerator<GroupRenderDetails> GetEnumerator()
        {
            return _groupDetails.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddGroup(GroupRenderDetails details)
        {
            _groupDetails.Add(details);
        }
    }
}