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
using System.Diagnostics;
using System.Numerics;
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

    internal sealed class SystemManager
    {
        private readonly EntityComponentSystemManager _manager;
        private SystemLayer[] _updateLayers;
        private SystemLayer[] _drawLayers;
        private readonly SystemLayer _dummyLayer;
        private bool _hasInitialized;

        internal List<EntitySystem> Systems;
        internal List<EntityProcessingSystem> ProcessingSystems;

        internal SystemManager(EntityComponentSystemManager manager)
        {
            _manager = manager;
            _updateLayers = new SystemLayer[0];
            _drawLayers = new SystemLayer[0];
            _dummyLayer  = new SystemLayer();
            Systems = new List<EntitySystem>();
            ProcessingSystems = new List<EntityProcessingSystem>();
        }

        internal void InitializeIfNecessary()
        {
            if (_hasInitialized)
                return;

            _hasInitialized = true;

            foreach (var system in Systems)
            {
                system.Initialize();
                system.LoadContent();
            }
        }

        internal void Update(GameTime gameTime)
        {
            ProcessLayers(gameTime, _updateLayers);
        }

        internal void Draw(GameTime gameTime)
        {
            ProcessLayers(gameTime, _drawLayers);
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void ProcessLayers(GameTime gameTime, SystemLayer[] layers)
        {
            Debug.Assert(layers != null);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < layers.Length; ++i)
            {
                var system = layers[i];
                if (system.SynchronousSystems.Count > 0)
                    ProcessSystemsSynchronous(gameTime, system.SynchronousSystems);
                //if (system.AsynchronousSystems.TotalCount > 0)
                //    ProcessSystemsAsynchronous(gameTime, system.AsynchronousSystems);
            }
        }

        private static void ProcessSystemsSynchronous(GameTime gameTime, Bag<EntitySystem> systems)
        {
            Debug.Assert(systems != null);

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

        internal T AddSystem<T>(T system, GameLoopType gameLoopType, int layer, SystemExecutionType executionType) where T : EntitySystem
        {
            Debug.Assert(system != null);

            return (T)AddSystem(system.GetType(), system, gameLoopType, layer, executionType);
        }

        private EntitySystem AddSystem(Type systemType, EntitySystem system, GameLoopType gameLoopType, int layer, SystemExecutionType executionType)
        {
            Debug.Assert(systemType != null);
            Debug.Assert(system != null);

            if (Systems.Contains(system))
                throw new InvalidOperationException($"System '{systemType}' has already been added.");

            system.Manager = _manager;

            Systems.Add(system);


            var processingSystem = system as EntityProcessingSystem;
            if (processingSystem != null)
            {
                processingSystem.BitIndex = ProcessingSystems.Count;
                ProcessingSystems.Add(processingSystem);
            }

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

        private void AddSystemTo(ref SystemLayer[] layers, EntitySystem system, int layerIndex, SystemExecutionType executionType)
        {
            Debug.Assert(layers != null);

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
        }

        private sealed class SystemLayer : IComparable<SystemLayer>
        {
            public int LayerIndex;

            public readonly Bag<EntitySystem> SynchronousSystems;
            //public readonly Bag<System> AsynchronousSystems;

            public SystemLayer(int layerIndex = 0)
            {
                LayerIndex = layerIndex;
                //AsynchronousSystems = new Bag<System>();
                SynchronousSystems = new Bag<EntitySystem>();
            }

            public int CompareTo(SystemLayer other)
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                return LayerIndex.CompareTo(other.LayerIndex);
            }
        }
    }
}
