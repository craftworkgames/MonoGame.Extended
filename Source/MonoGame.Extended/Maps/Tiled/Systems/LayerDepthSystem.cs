using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Maps.Tiled.Components;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.Maps.Tiled.Systems
{
    public class LayerDepthSystem : ComponentSystem
    {
        public override void Update(GameTime gameTime)
        {
            var components = GetComponents<DepthComponent>();
            var max = components.Max(x => x.Level);
            foreach (var component in components)
            {
                component.Depth = max == 0 ? 0 : 1 - component.Level / max;
                var componentSprites = component.Entity.GetComponents<Sprite>();
                foreach (var sprite in componentSprites)
                    sprite.Depth = component.Depth;
            }
        }
    }
}
