// Original code dervied from:
// https://github.com/thelinuxlich/starwarrior_CSharp/blob/master/StarWarrior/StarWarrior/Systems/CollisionSystem.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollisionSystem.cs" company="GAMADU.COM">
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
//   The collision system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Demo.StarWarrior.Components;
using Demo.StarWarrior.Templates;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Demo.StarWarrior.Systems
{
    [EntitySystem(GameLoopType.Update, Layer = 1)]
    public class CollisionSystem : EntitySystem
    {
        protected override void Process(GameTime gameTime)
        {
            var bullets = EntityManager.GetEntitiesByGroup("BULLETS");
            var ships = EntityManager.GetEntitiesByGroup("SHIPS");
            if (bullets == null || ships == null)
                return;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var shipIndex = 0; ships.Count > shipIndex; ++shipIndex)
            {
                var ship = ships[shipIndex];

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var bulletIndex = 0; bullets.Count > bulletIndex; ++bulletIndex)
                {
                    var bullet = bullets[bulletIndex];
                    var bulletTransform = bullet.Get<TransformComponent>();
                    var shipTransform = ship.Get<TransformComponent>();

                    if (!CollisionExists(bulletTransform, shipTransform))
                        continue;

                    var bulletExplosion = EntityManager.CreateEntityFromTemplate(BulletExplosionTemplate.Name);
                    bulletExplosion.Get<TransformComponent>().Position = bulletTransform.Position;
                    bullet.Destroy();

                    var healthComponent = ship.Get<HealthComponent>();
                    healthComponent.AddDamage(4);

                    if (healthComponent.IsAlive)
                        continue;
                  
                    var shipExplosion = EntityManager.CreateEntityFromTemplate(ShipExplosionTemplate.Name);
                    shipExplosion.Get<TransformComponent>().Position = shipTransform.Position;
                    ship.Destroy();
                    break;
                }
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static bool CollisionExists(TransformComponent bulletTransform, TransformComponent shipTransform)
        {
            return Vector2.Distance(bulletTransform.WorldPosition, shipTransform.WorldPosition) < 20;
        }
    }
}