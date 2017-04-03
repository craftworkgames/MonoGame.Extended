// Original code dervied from:
// https://github.com/thelinuxlich/starwarrior_CSharp/blob/master/StarWarrior/StarWarrior/Systems/EnemyShooterSystem.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnemyShooterSystem.cs" company="GAMADU.COM">
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
//   The enemy shooter system.

// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Demo.StarWarrior.Components;
using Demo.StarWarrior.Templates;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Demo.StarWarrior.Systems
{
    [Aspect(AspectType.All, typeof(WeaponComponent), typeof(TransformComponent), typeof(EnemyComponent))]
    [EntitySystem(GameLoopType.Update, Layer = 1)]
    public class EnemyShooterSystem : EntityProcessingSystem
    {
        protected override void Process(GameTime gameTime, Entity entity)
        {
            var transform = entity.Get<TransformComponent>();
            var weapon = entity.Get<WeaponComponent>();
                
            weapon.ShootTimerDelay += gameTime.ElapsedGameTime;
            if (weapon.ShootTimerDelay <= weapon.ShootDelay)
                return;
            weapon.ShootTimerDelay -= weapon.ShootDelay;

            var missile = EntityManager.CreateEntityFromTemplate(MissileTemplate.Name);
            var missileTransform = missile.Get<TransformComponent>();

            var worldPosition = transform.WorldPosition;
            missileTransform.Position = worldPosition + new Vector2(0, 20);

            var missilePhysics = missile.Get<PhysicsComponent>();

            missilePhysics.Speed = -0.5f;
            missilePhysics.Angle = 270;
        }
    }
}