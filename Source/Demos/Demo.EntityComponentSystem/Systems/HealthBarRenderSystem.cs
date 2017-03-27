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

using Demo.StarWarriorGame.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;

namespace Demo.StarWarriorGame.Systems
{
    [System(
        AspectType = AspectType.AllOf,
        ComponentTypes = new[] { typeof(HealthComponent), typeof(TransformComponent) },
        GameLoopType = GameLoopType.Draw,
        Layer = 0)]
    public class HealthBarRenderSystem : EntityProcessingSystem<HealthComponent, TransformComponent>
    {
        private BitmapFont _font;
        private SpriteBatch _spriteBatch;

        public override void LoadContent()
        {
            _spriteBatch = Game.Services.GetService<SpriteBatch>();
            _font = Game.Services.GetService<BitmapFont>();
        }

        protected override void Process(GameTime gameTime, Entity entity, HealthComponent health, TransformComponent transform)
        {
            var text = health.HealthPercentage + "%";
            var c = (float)(health.HealthPercentage / 100);
            var color = new Color(1.0f - c, c, 0.0f);
            var worldPosition = transform.WorldPosition;
            var renderPosition = worldPosition + new Vector2(0, 25);
            _spriteBatch.DrawString(_font, text, renderPosition, color, 0.0f, _font.MeasureString(text) * 0.5f, 1.0f, SpriteEffects.None, 0.5f);
        }
    }
}