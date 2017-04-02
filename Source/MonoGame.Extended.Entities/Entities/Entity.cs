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
using System.Runtime.CompilerServices;
using MonoGame.Extended.Collections;
// ReSharper disable InconsistentNaming

namespace MonoGame.Extended.Entities
{
    public sealed class Entity : IPoolable
    {
        private static readonly uint ToBeAddedMask;
        private static readonly uint ToBeRemovedMask;
        private static readonly uint ToRefreshComponentsMask;

        static Entity()
        {
            ToBeAddedMask = BitVector32.CreateMask();
            ToBeRemovedMask = BitVector32.CreateMask(ToBeAddedMask);
            ToRefreshComponentsMask = BitVector32.CreateMask(ToBeRemovedMask);
        }

        internal EntityManager Manager;
        internal BitVector SystemBits;
        internal BitVector ComponentBits;
        private ReturnToPoolDelegate _returnToPoolDelegate;
        internal BitVector32 Flags;
        internal string _group;
        internal string _name;

        public bool IsActive
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return !Flags[ToBeRemovedMask]; }
        }

        public bool WaitingToBeAdded
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Flags[ToBeAddedMask]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal set { Flags[ToBeAddedMask] = value; }
        }

        public bool WaitingToBeRemoved
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Flags[ToBeRemovedMask]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal set { Flags[ToBeRemovedMask] = value; }
        }

        internal bool WaitingToRefreshComponents
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Flags[ToRefreshComponentsMask]; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Flags[ToRefreshComponentsMask] = value; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Manager.AddEntityName(value, this);
            }
        }

        public string Group
        {
            get { return _group; }
            set
            {
                _group = value;
                Manager.AddEntityToGroup(value, this);
            }
        }

        IPoolable IPoolable.NextNode { get; set; }
        IPoolable IPoolable.PreviousNode { get; set; }

        internal Entity(int systemTypeCount, int componentTypeCount)
        {
            SystemBits = new BitVector(systemTypeCount);
            ComponentBits = new BitVector(componentTypeCount);
        }

        public void Destroy()
        {
            Manager.MarkEntityToBeRemoved(this);
        }

        public T Attach<T>(Action<T> configure) where T : EntityComponent
        {
            var component = Attach<T>();
            configure(component);
            return component;
        }

        public T Attach<T>() where T : EntityComponent
        {
            return Manager.AddComponent<T>(this);
        }

        public void Detach<T>() where T : EntityComponent
        {
            Manager.MarkComponentToBeRemoved<T>(this);
        }

        public T Get<T>() where T : EntityComponent
        {
            return Manager.GetComponent<T>(this);
        }

        internal void Reset()
        {
            _group = null;
            SystemBits.SetAll(false);
            ComponentBits.SetAll(false);
            Flags = 0;
        }

        public override string ToString()
        {
            return $"{RuntimeHelpers.GetHashCode(this):X8}";
        }

        void IPoolable.Initialize(ReturnToPoolDelegate returnDelegate)
        {
            Reset();
            _returnToPoolDelegate = returnDelegate;
        }

        void IPoolable.Return()
        {
            Return();
        }

        internal void Return()
        {
            Reset();

            if (_returnToPoolDelegate == null)
            {
                return;
            }

            _returnToPoolDelegate.Invoke(this);
            _returnToPoolDelegate = null;
        }
    }
}