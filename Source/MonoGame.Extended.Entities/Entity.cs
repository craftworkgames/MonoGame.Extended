// Original code dervied from:
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/Entity.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="GAMADU.COM">
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
//   Basic unity of this entity system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public sealed class Entity
    {
        private readonly EntityComponentSystemManager _manager;
        internal BigInteger SystemBits;
        internal BigInteger TypeBits;

        internal int Index;
        internal Guid Identifier;

        public bool IsEnabled { get; internal set; }
        public bool MarkedForRemoval { get; internal set; }
        public bool IsRefreshing { get; internal set; }

        public string Group
        {
            get
            {
                return _manager.GetEntityGroup(this);
            }
            set
            {
                _manager.SetGroupForEntity(value, this);
            }
        }

        public bool IsGrouped => _manager.GetEntityGroup(this) != null;
        public bool IsActive => _manager.IsActive(this);

        internal Entity(EntityComponentSystemManager manager, int index, Guid identifier)
        {
            Index = index;
            Identifier = identifier;
            SystemBits = 0;
            TypeBits = 0;
            IsEnabled = true;
            _manager = manager;
        }

        public T AddComponent<T>() where T : Component
        {
            var component = _manager.AddComponent<T>(this);
            return component;
        }

        public void Delete()
        {
            _manager.RemoveEntityFromGroupIfPossible(this);
        }

        public T GetComponent<T>() where T : Component
        {
            return _manager.GetComponent<T>(this);
        }

        public bool HasComponent<T>() where T : Component
        {
            return _manager.GetComponent(this, ComponentType<T>.Type) == null;
        }

        public void Refresh()
        {
            _manager.Refresh(this);
        }

        public void RemoveComponent<T>() where T : Component
        {
            _manager.RemoveComponent(this, ComponentTypeManager.GetTypeFor<T>());
        }

        public void RemoveComponent(ComponentType componentType)
        {
            _manager.RemoveComponent(this, componentType);
        }

        public void Reset()
        {
            SystemBits = 0;
            TypeBits = 0;
            IsEnabled = true;
        }

        public override string ToString()
        {
            return $"Entity{{{Identifier}}}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddSystemBit(BigInteger bit)
        {
            SystemBits |= bit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void AddTypeBit(BigInteger bit)
        {
            TypeBits |= bit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveSystemBit(BigInteger bit)
        {
            SystemBits &= ~bit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void RemoveTypeBit(BigInteger bit)
        {
            TypeBits &= ~bit;
        }
    }
}