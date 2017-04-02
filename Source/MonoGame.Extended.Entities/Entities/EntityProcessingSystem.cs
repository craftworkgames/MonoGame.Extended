// Original code derived from: 
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/System/EntityProcessingSystem.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityProcessingSystem.cs" company="GAMADU.COM">
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
//   Class EntityProcessingSystem.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities
{
    public abstract class EntityProcessingSystem : EntitySystem
    {
        private readonly Dictionary<Entity, int> _activeEntitiesLookup = new Dictionary<Entity, int>();
        // ReSharper disable once InconsistentNaming
        internal readonly List<Entity> _activeEntities = new List<Entity>();
        internal int BitIndex;
        internal Aspect Aspect;

        public IEnumerable<Entity> ActiveEntities => _activeEntities;

        protected EntityProcessingSystem()
        {
        }

        internal virtual void RefreshEntityComponents(Entity entity)
        {
            var isInterested = Aspect.Matches(entity.ComponentBits);
            if (!isInterested)
                return;

            var contains = entity.SystemBits[BitIndex];

            if (!contains && entity.IsActive)
            {
                Add(entity);
            }
            else if (contains && !entity.IsActive)
            {
                Remove(entity);
            }
        }
        public virtual void OnEntityAdded(Entity entity)
        {
        }

        public virtual void OnEntityDisabled(Entity entity)
        {
        }

        public virtual void OnEntityEnabled(Entity entity)
        {
        }

        public virtual void OnEntityRemoved(Entity entity)
        {
        }

        protected override void Process(GameTime gameTime)
        {
            base.Process(gameTime);

            for (var i = _activeEntities.Count - 1; i >= 0; i--)
            {
                var entity = _activeEntities[i];
                Process(gameTime, entity);
            }
        }

        protected virtual void Process(GameTime gameTime, Entity entity)
        {
        }

        protected bool IsInterestedIn(Entity entity)
        {
            return Aspect.Matches(entity.ComponentBits);
        }

        internal void Add(Entity entity)
        {
            if (entity == null)
                return;

            entity.SystemBits[BitIndex] = true;

            if (_activeEntitiesLookup.ContainsKey(entity))
                return;

            _activeEntitiesLookup.Add(entity, _activeEntities.Count);
            _activeEntities.Add(entity);

            OnEntityAdded(entity);
        }

        internal void Remove(Entity entity)
        {
            if (entity == null)
                return;

            entity.SystemBits[BitIndex] = false;

            int activeEntityIndex;
            if (!_activeEntitiesLookup.TryGetValue(entity, out activeEntityIndex))
                return;
            _activeEntitiesLookup.Remove(entity);

            var swapEntity = _activeEntities[_activeEntities.Count - 1];

            if (entity != swapEntity)
                _activeEntitiesLookup[swapEntity] = activeEntityIndex;

            _activeEntities[activeEntityIndex] = swapEntity;
            _activeEntities.RemoveAt(_activeEntities.Count - 1);

            OnEntityRemoved(entity);
        }
    }  
}
