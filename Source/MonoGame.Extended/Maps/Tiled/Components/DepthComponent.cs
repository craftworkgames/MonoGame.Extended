using MonoGame.Extended.Entities.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.Maps.Tiled.Components
{
    public class DepthComponent : EntityComponent
    {
        public DepthComponent()
        {
            Depth = 0.0f;
            Level = 0;
        }

        public float Depth;
        public float Level;
    }
}
