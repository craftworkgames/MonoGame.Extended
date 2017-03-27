//// Original code dervied from:
//// https://github.com/thelinuxlich/starwarrior_CSharp/blob/master/StarWarrior/StarWarrior/Systems/PlayerShipControlSystem.cs

//// --------------------------------------------------------------------------------------------------------------------
//// <copyright file="PlayerShipControlSystem.cs" company="GAMADU.COM">
////     Copyright © 2013 GAMADU.COM. All rights reserved.
////
////     Redistribution and use in source and binary forms, with or without modification, are
////     permitted provided that the following conditions are met:
////
////        1. Redistributions of source code must retain the above copyright notice, this list of
////           conditions and the following disclaimer.
////
////        2. Redistributions in binary form must reproduce the above copyright notice, this list
////           of conditions and the following disclaimer in the documentation and/or other materials
////           provided with the distribution.
////
////     THIS SOFTWARE IS PROVIDED BY GAMADU.COM 'AS IS' AND ANY EXPRESS OR IMPLIED
////     WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
////     FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL GAMADU.COM OR
////     CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
////     CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
////     SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
////     ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
////     NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
////     ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////
////     The views and conclusions contained in the software and documentation are those of the
////     authors and should not be interpreted as representing official policies, either expressed
////     or implied, of GAMADU.COM.
//// </copyright>
//// <summary>
////   The player ship control system.
//// </summary>
//// --------------------------------------------------------------------------------------------------------------------

//using System;
//using System.Diagnostics.CodeAnalysis;
//using Demo.StarWarriorGame.Components;
//using Demo.StarWarriorGame.Templates;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Input;
//using MonoGame.Extended.Entities;

//namespace Demo.StarWarriorGame.Systems
//{
//    [System(
//        AspectType = AspectType.AllOf,
//        ComponentTypes = new[] { typeof(PlayerComponent), typeof(TransformComponent) },
//        GameLoopType = GameLoopType.Update,
//        Layer = 0)]
//    public class PlayerShipControlSystem : EntityProcessingSystem<PlayerComponent, TransformComponent>
//    {
//        private TimeSpan _missileLaunchTimer;
//        private readonly TimeSpan _missileLaunchDelay;

//        public PlayerShipControlSystem()
//        {
//            _missileLaunchDelay = TimeSpan.FromMilliseconds(150);
//            _missileLaunchTimer = TimeSpan.Zero;
//        }

//        [SuppressMessage("ReSharper", "InvertIf")]
//        protected override void Process(GameTime gameTime, Entity entity, PlayerComponent player, TransformComponent transform)
//        {
//            var keyboard = Keyboard.GetState();
//            var keyMoveSpeed = 0.3f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

//            var positionDelta = new Vector2();

//            var viewport = GraphicsDevice.Viewport;

//            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
//            {
//                positionDelta.X -= keyMoveSpeed;
//                if (positionDelta.X < 32)
//                {
//                    positionDelta.X = 32;
//                }
//            }
//            else if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
//            {
//                positionDelta.X += keyMoveSpeed;
//                if (positionDelta.X > viewport.Width - 32)
//                {
//                    positionDelta.X = viewport.Width - 32;
//                }
//            }

//            if (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter))
//            {
//                _missileLaunchTimer += gameTime.ElapsedGameTime;
//                if (_missileLaunchTimer <= _missileLaunchDelay)
//                {
//                    _missileLaunchTimer -= _missileLaunchDelay;

//                    AddMissile(transform);
//                    AddMissile(transform, 89, -9);
//                    AddMissile(transform, 91, +9);
//                }
//            }

//            transform.Position += positionDelta;
//        }

//        private void AddMissile(TransformComponent parentTransform, float angle = 90.0f, float offsetX = 0.0f)
//        {
//            var missile = Manager.NewFromTemplate(MissileTemplate.Name);

//            var missileTransform = missile.GetEntityByName<TransformComponent>();
//            missileTransform.Position = parentTransform.WorldPosition + new Vector2(1 + offsetX, -20);

//            var missilePhysics = missile.GetEntityByName<PhysicsComponent>();
//            missilePhysics.Speed = -0.5f;
//            missilePhysics.Angle = angle;
//        }
//    }
//}