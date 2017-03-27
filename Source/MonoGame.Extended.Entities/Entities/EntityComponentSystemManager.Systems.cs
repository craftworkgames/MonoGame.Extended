// Original code dervied from:
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/Manager/SystemManager.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManager.cs" company="GAMADU.COM">
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
//   Class SystemManager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public enum GameLoopType
    {
        Update,
        Draw
    }

    public enum SystemExecutionType
    {
        Synchronous,
        //Asynchronous,
    }

    public sealed partial class EntityComponentSystemManager
    {
        private readonly Dictionary<System, BigInteger> _systemBits = new Dictionary<System, BigInteger>();
        private int _systemBitPositionIndex;
        private SystemLayer[] _updateLayers = new SystemLayer[0];
        private SystemLayer[] _drawLayers = new SystemLayer[0];
        private readonly SystemLayer _dummyLayer = new SystemLayer();

        public Bag<System> Systems { get; } = new Bag<System>();

        public BigInteger GetBitFor(System system)
        {
            BigInteger bit;
            if (_systemBits.TryGetValue(system, out bit))
                return bit;
            bit = new BigInteger(1) << _systemBitPositionIndex;
            ++_systemBitPositionIndex;
            _systemBits.Add(system, bit);
            return bit;
        }

        internal void UpdateSystems(GameTime gameTime)
        {
            ProcessLayers(gameTime, _updateLayers);
        }

        internal void DrawSystems(GameTime gameTime)
        {
            ProcessLayers(gameTime, _drawLayers);
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void ProcessLayers(GameTime gameTime, SystemLayer[] layers)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < layers.Length; ++i)
            {
                var system = layers[i];
                if (system.SynchronousSystems.Count > 0)
                    ProcessSystemsSynchronous(gameTime, system.SynchronousSystems);
                //if (system.AsynchronousSystems.Count > 0)
                //    ProcessSystemsAsynchronous(gameTime, system.AsynchronousSystems);
            }
        }

        private static void ProcessSystemsSynchronous(GameTime gameTime, Bag<System> systems)
        {
            for (int index = 0, j = systems.Count; index < j; ++index)
                systems[index].ProcessInternal(gameTime);
        }

        //// ReSharper disable once ParameterTypeCanBeEnumerable.Local
        //private static void ProcessSystemsAsynchronous(Bag<System> systems)
        //{
        //    // block
        //    // does not garantee async...
        //    Parallel.ForEach(systems, system => system.ProcessInternal());
        //}

        internal T AddSystem<T>(T system, GameLoopType gameLoopType, int layer = 0, SystemExecutionType executionType = SystemExecutionType.Synchronous) where T : System
        {
            return (T)AddSystem(system.GetType(), system, gameLoopType, layer, executionType);
        }

        private System AddSystem(Type systemType, System system, GameLoopType gameLoopType, int layer = 0, SystemExecutionType executionType = SystemExecutionType.Synchronous)
        {
            if (Systems.Contains(system))
                throw new InvalidOperationException($"System '{systemType}' has already been added.");

            system.Bit = GetBitFor(system);

            switch (gameLoopType)
            {
                case GameLoopType.Draw:
                    AddSystemTo(ref _drawLayers, system, layer, executionType);
                    break;
                case GameLoopType.Update:
                    AddSystemTo(ref _updateLayers, system, layer, executionType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameLoopType), gameLoopType, null);
            }

            return system;
        }

        private void AddSystemTo(ref SystemLayer[] layers, System system, int layerIndex, SystemExecutionType executionType)
        {
            _dummyLayer.LayerIndex = layerIndex;
            var index = Array.BinarySearch(layers, _dummyLayer);
            SystemLayer layer;
            if (index >= 0)
            {
                layer = layers[index];
            }
            else
            {
                layer = new SystemLayer(layerIndex);
                Array.Resize(ref layers, layers.Length + 1);
                layers[layers.Length - 1] = layer;
                Array.Sort(layers);
            }

            switch (executionType)
            {
                case SystemExecutionType.Synchronous:
                    layer.SynchronousSystems.Add(system);
                    break;
                //case SystemExecutionType.Asynchronous:
                //    layer.AsynchronousSystems.Attach(system);
                //    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(executionType), executionType, null);
            }

            system.Manager = this;
            Systems.Add(system);
        }

        private sealed class SystemLayer : IComparable<SystemLayer>
        {
            public int LayerIndex;

            public readonly Bag<System> SynchronousSystems;
            //public readonly Bag<System> AsynchronousSystems;

            public SystemLayer(int layerIndex = 0)
            {
                LayerIndex = layerIndex;
                //AsynchronousSystems = new Bag<System>();
                SynchronousSystems = new Bag<System>();
            }

            public int CompareTo(SystemLayer other)
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                return LayerIndex.CompareTo(other.LayerIndex);
            }
        }
    }
}
