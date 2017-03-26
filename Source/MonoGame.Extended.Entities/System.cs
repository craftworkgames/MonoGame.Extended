// Original code dervied from:
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/System/EntitySystem.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySystem.cs" company="GAMADU.COM">
//     Copyright © 2013 GAMADU.COM. Contains rights reserved.
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
//   Base of all Entity Systems. Provide basic functionalities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public abstract class System
    {
        private readonly Dictionary<int, int> _activeEntitiesLookup = new Dictionary<int, int>();
        private readonly Bag<Entity> _activeEntities = new Bag<Entity>();
        internal BigInteger Bit;
        internal Aspect Aspect;

        public EntityComponentSystemManager Manager { get; internal set; }
        public bool IsEnabled { get; set; }
        public IEnumerable<Entity> ActiveEntities => _activeEntities;
        public Game Game => Manager.Game;
        public GraphicsDevice GraphicsDevice => Manager.GraphicsDevice;

        protected System()
        {
            IsEnabled = true;
        }

        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void UnloadContent()
        {
        }

        public virtual void OnEntityAdded(Entity entity)
        {
        }

        public virtual void OnEntityChanged(Entity entity)
        {
            var contains = (Bit & entity.SystemBits) == Bit;
            var isInterested = Aspect.IsInterestedIn(entity);

            if (isInterested && !contains)
            {
                Add(entity);
            }
            else if (!isInterested && contains)
            {
                Remove(entity);
            }
            else if (isInterested && entity.IsEnabled)
            {
                Enable(entity);
            }
            else if (isInterested && !entity.IsEnabled)
            {
                Disable(entity);
            }
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

        internal void Process(GameTime gameTime)
        {
            if (!CheckProcessing(gameTime))
                return;
            Begin(gameTime);
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = _activeEntities.Count - 1; index >= 0; --index)
                Process(gameTime, _activeEntities[index]);
            End(gameTime);
        }

        public void Toggle()
        {
            IsEnabled = !IsEnabled;
        }

        protected virtual void Begin(GameTime gameTime)
        {
        }

        protected virtual bool CheckProcessing(GameTime gameTime)
        {
            return IsEnabled;
        }

        protected virtual void End(GameTime gameTime)
        {
        }

        protected virtual bool IsInterestedIn(Entity entity)
        {
            return Aspect.IsInterestedIn(entity);
        }

        protected virtual void Process(GameTime gameTime, Entity entity)
        {
        }

        protected void Add(Entity entity)
        {
            if (entity == null)
                return;

            entity.AddSystemBit(Bit);
            if (entity.IsEnabled)
                Enable(entity);

            OnEntityAdded(entity);
        }

        protected void Remove(Entity entity)
        {
            if (entity == null)
                return;

            entity.RemoveSystemBit(Bit);
            if (entity.IsEnabled)
                Disable(entity);

            OnEntityRemoved(entity);
        }

        private void Disable(Entity entity)
        {
            if (entity == null)
                return;
            int activeEntityIndex;
            if (!_activeEntitiesLookup.TryGetValue(entity.Index, out activeEntityIndex))
                return;
            _activeEntitiesLookup.Remove(entity.Index);

            if (_activeEntities.Count > 0)
            {
                var swapEntity = _activeEntities[_activeEntities.Count - 1];
                _activeEntitiesLookup[swapEntity.Index] = activeEntityIndex;
            }

            _activeEntities.Remove(activeEntityIndex);
            OnEntityDisabled(entity);
        }

        private void Enable(Entity entity)
        {
            if (entity == null || _activeEntitiesLookup.ContainsKey(entity.Index))
                return;

            _activeEntitiesLookup.Add(entity.Index, _activeEntities.Count);
            _activeEntities.Add(entity);
            OnEntityEnabled(entity);
        }
    }
}