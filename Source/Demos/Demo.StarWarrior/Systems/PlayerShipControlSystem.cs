// Original code dervied from:
// https://github.com/thelinuxlich/starwarrior_CSharp/blob/master/StarWarrior/StarWarrior/Systems/PlayerShipControlSystem.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerShipControlSystem.cs" company="GAMADU.COM">
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
//   The player ship control system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Demo.StarWarrior.Components;
using Demo.StarWarrior.Templates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using MonoGame.Extended.BitmapFonts;

namespace Demo.StarWarrior.Systems
{
    [Aspect(AspectType.All, typeof(PlayerComponent), typeof(TransformComponent))]
    [EntitySystem(GameLoopType.Update, Layer = 0)]
    public class PlayerShipControlSystem : EntityProcessingSystem
    {
        private TimeSpan _missileLaunchTimer;
        private readonly TimeSpan _missileLaunchDelay;
        private KeyboardState _lastState;

        public PlayerShipControlSystem()
        {
            _missileLaunchDelay = TimeSpan.FromMilliseconds(250);
            _missileLaunchTimer = TimeSpan.Zero;
        }

        [SuppressMessage("ReSharper", "InvertIf")]
        protected override void Process(GameTime gameTime, Entity entity)
        {
            var transform = entity.Get<TransformComponent>();

            var keyboard = Keyboard.GetState();
            var direction = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
                direction -= Vector2.UnitY;

            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
                direction -= Vector2.UnitX;

            if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
                direction += Vector2.UnitY;

            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
                direction += Vector2.UnitX;

            if (keyboard.IsKeyDown(Keys.K) && !_lastState.IsKeyDown(Keys.K))
            {
                BitmapFont.UseKernings = !BitmapFont.UseKernings;
            }

            var isMoving = direction != Vector2.Zero;
            if (isMoving)
            {
                direction.Normalize();
            }

            var speed = 400;
            transform.Position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter))
            {
                _missileLaunchTimer += gameTime.ElapsedGameTime;
                if (_missileLaunchTimer <= _missileLaunchDelay)
                {
                    _missileLaunchTimer -= _missileLaunchDelay;

                    AddMissile(transform);
                    AddMissile(transform, 89, -9);
                    AddMissile(transform, 91, +9);
                }
            }

            _lastState = keyboard;
        }

        private void AddMissile(TransformComponent parentTransform, float angle = 90.0f, float offsetX = 0.0f)
        {
            var missile = EntityManager.CreateEntityFromTemplate(MissileTemplate.Name);

            var missileTransform = missile.Get<TransformComponent>();
            missileTransform.Position = parentTransform.WorldPosition + new Vector2(1 + offsetX, -20);

            var missilePhysics = missile.Get<PhysicsComponent>();
            missilePhysics.Speed = -0.5f;
            missilePhysics.Angle = angle;
        }
    }
}