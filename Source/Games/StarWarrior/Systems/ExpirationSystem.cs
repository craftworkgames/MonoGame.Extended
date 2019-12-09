// Original code dervied from:
// https://github.com/thelinuxlich/starwarrior_CSharp/blob/master/StarWarrior/StarWarrior/Systems/ExpirationSystem.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpirationSystem.cs" company="GAMADU.COM">
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
//   The expiration system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using StarWarrior.Components;

namespace StarWarrior.Systems
{
    public class ExpirationSystem : EntityProcessingSystem
    {
        public ExpirationSystem() 
            : base(Aspect.All(typeof(ExpiresComponent)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var entity = GetEntity(entityId);
            var expires = entity.Get<ExpiresComponent>();

            expires.LifeTime -= gameTime.ElapsedGameTime;
            if (expires.LifeTime > TimeSpan.Zero)
                return;

            entity.Destroy();
            expires.LifeTime = TimeSpan.Zero;
        }
    }
}