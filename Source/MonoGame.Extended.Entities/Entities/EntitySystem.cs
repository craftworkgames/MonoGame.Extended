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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Entities
{
    public abstract class EntitySystem
    {
        private TimeSpan _timer;

        internal EntityComponentSystemManager Manager;
        public bool IsEnabled { get; set; }
        public Game Game => Manager.Game;
        public GraphicsDevice GraphicsDevice => Manager.GraphicsDevice;
        public EntityManager EntityManager => Manager.EntityManager;

        public TimeSpan ProcessingDelay { get; set; }

        protected EntitySystem()
        {
            IsEnabled = true;
            _timer = TimeSpan.Zero;
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

        internal void ProcessInternal(GameTime gameTime)
        {
            if (!CheckProcessing(gameTime))
                return;
            Begin(gameTime);
            Process(gameTime);
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
            // ReSharper disable once InvertIf
            if (ProcessingDelay != TimeSpan.Zero)
            {
                _timer += gameTime.ElapsedGameTime;
                if (_timer <= ProcessingDelay)
                    return false;
                _timer -= ProcessingDelay;
            }
            return IsEnabled;
        }

        protected virtual void Process(GameTime gameTime)
        {
        }

        protected virtual void End(GameTime gameTime)
        {
        }
    }
}