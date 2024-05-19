using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Extended.Triangulation
{
    /// <summary>
	/// Implements a List structure as a cyclical list where indices are wrapped.
	/// </summary>
    /// MIT Licensed: https://github.com/nickgravelyn/Triangulator
	/// <typeparam name="T">The Type to hold in the list.</typeparam>
	class CyclicalList<T> : List<T>
    {
        public new T this[int index]
        {
            get
            {
                //perform the index wrapping
                while (index < 0)
                    index = Count + index;
                if (index >= Count)
                    index %= Count;

                return base[index];
            }
            set
            {
                //perform the index wrapping
                while (index < 0)
                    index = Count + index;
                if (index >= Count)
                    index %= Count;

                base[index] = value;
            }
        }

        public CyclicalList() { }

        public CyclicalList(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public new void RemoveAt(int index)
        {
            Remove(this[index]);
        }
    }
}
