// Original code dervied from:
// https://github.com/thelinuxlich/starwarrior_CSharp/blob/master/StarWarrior/StarWarrior/Systems/HealthBarRenderSystem.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthBarRenderSystem.cs" company="GAMADU.COM">
//     Copyright � 2013 GAMADU.COM. All rights reserved.
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
//   The health bar render system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using StarWarrior.Components;

namespace StarWarrior.Systems
{
    public class HealthBarRenderSystem : EntityDrawSystem
    {
        private readonly BitmapFont _font;
        private readonly SpriteBatch _spriteBatch;
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public HealthBarRenderSystem(SpriteBatch spriteBatch, BitmapFont font) 
            : base(Aspect.All(typeof(HealthComponent), typeof(Transform2)))
        {
            _spriteBatch = spriteBatch;
            _font = font;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entityId in ActiveEntities)
            {
                var entity = GetEntity(entityId);
                var health = entity.Get<HealthComponent>();
                var transform = entity.Get<Transform2>();

                _stringBuilder.Clear();
                _stringBuilder.Append((float)Math.Round(health.Ratio * 100, 1));
                _stringBuilder.Append("%");

                var c = health.Ratio / 1;
                var color = new Color(1.0f - c, c, 0.0f);
                var worldPosition = transform.WorldPosition;
                var renderPosition = worldPosition + new Vector2(0, 25);

                var stringSize = _font.MeasureString(_stringBuilder) * 0.5f;
                _spriteBatch.DrawString(_font, _stringBuilder, renderPosition, color, 0.0f, stringSize, 1.0f, SpriteEffects.None, 0.5f);
            }
        }
    }
}