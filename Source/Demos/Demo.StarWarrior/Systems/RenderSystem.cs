// Original code dervied from:
// https://github.com/thelinuxlich/starwarrior_CSharp/blob/master/StarWarrior/StarWarrior/Systems/RenderSystem.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderSystem.cs" company="GAMADU.COM">
//     Copyright © 2013 GAMADU.COM. All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without modification, are
//     permitted provided that the following conditions are met:
//
//        1. Redistributions of source code must retain the above copyright notice, this list of
//           conditions and the following disclaimer.
//
//        2. Redistributions in binary form must reproduce the above copyright notice, this list
//           of conditions and the following disclaimer in the documentation and/or other materials
//           provided with the distribution.
//
//     THIS SOFTWARE IS PROVIDED BY GAMADU.COM 'AS IS' AND ANY EXPRESS OR IMPLIED
//     WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//     FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL GAMADU.COM OR
//     CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//     CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//     SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//     ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//     NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//     ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     The views and conclusions contained in the software and documentation are those of the
//     authors and should not be interpreted as representing official policies, either expressed
//     or implied, of GAMADU.COM.
// </copyright>
// <summary>
//   The render system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Demo.StarWarrior.Components;
using Demo.StarWarrior.Spatials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;

namespace Demo.StarWarrior.Systems
{
    [Aspect(AspectType.All, typeof(SpatialFormComponent), typeof(TransformComponent))]
    [EntitySystem(GameLoopType.Draw, Layer = 0)]
    public class RenderSystem : EntityProcessingSystem
    {
        private ContentManager _contentManager;
        private SpriteBatch _spriteBatch;

        public override void LoadContent()
        {
            _spriteBatch = Game.Services.GetService<SpriteBatch>();
            _contentManager = Game.Services.GetService<ContentManager>();
        }

        protected override void Process(GameTime gameTime, Entity entity)
        {
            var spatial = entity.Get<SpatialFormComponent>();
            var transform = entity.Get<TransformComponent>();

            var spatialName = spatial.SpatialFormFile;

            var worldPosition = transform.WorldPosition;
            if (!(worldPosition.X >= 0) || !(worldPosition.Y >= 0) ||
                !(worldPosition.X < _spriteBatch.GraphicsDevice.Viewport.Width) ||
                !(worldPosition.Y < _spriteBatch.GraphicsDevice.Viewport.Height))
                return;

            // very naive render ...
            if (string.Compare("PlayerShip", spatialName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                PlayerShip.Render(_spriteBatch, _contentManager, transform);
            }
            else if (string.Compare("Missile", spatialName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                Missile.Render(_spriteBatch, _contentManager, transform);
            }
            else if (string.Compare("EnemyShip", spatialName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                EnemyShip.Render(_spriteBatch, _contentManager, transform);
            }
            else if (string.Compare("BulletExplosion", spatialName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                Explosion.Render(_spriteBatch, _contentManager, transform, Color.Red, 10);
            }
            else if (string.Compare("ShipExplosion", spatialName, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                ShipExplosion.Render(_spriteBatch, _contentManager, transform, Color.Yellow, 30);
            }
        }
    }
}