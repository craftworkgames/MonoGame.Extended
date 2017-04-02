// Original code dervied from:
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/EntityWorld.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityWorld.cs" company="GAMADU.COM">
//     Copyright © 2013 GAMADU.COM. Contains rights reserved.
//     Redistribution and use in source and binary forms, with or without modification, are
//     permitted provided that the following conditions are met:
//        1. Redistributions of source code must retain the above copyright notice, this list of
//           conditions and the following disclaimer.
//        2. Redistributions in binary form must reproduce the above copyright notice, this list
//           of conditions and the following disclaimer in the documentation and/or other materials
//           provided with the distribution.
//     THIS SOFTWARE IS PROVIDED BY GAMADU.COM 'AS IS' AND ANY EXPRESS OR IMPLIED
//     WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//     FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL GAMADU.COM OR
//     CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//     CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//     SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//     ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//     NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//     ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//     The views and conclusions contained in the software and documentation are those of the
//     authors and should not be interpreted as representing official policies, either expressed
//     or implied, of GAMADU.COM.
// </copyright>
// <summary>
//   The Entity World Class. Main interface of the Entity System.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities
{
    public sealed class EntityComponentSystemManager : DrawableGameComponent
    {
        private readonly SystemManager _systemManager;

        public EntityManager EntityManager { get; }

        public EntityComponentSystemManager(Game game)
            : base(game)
        {
            _systemManager = new SystemManager(this);
            EntityManager = new EntityManager(_systemManager);
        }

        // don't call this every frame, lol
        public void Scan(params Assembly[] assemblies)
        {
            var componentTypeInfos = new List<TypeInfo>();
            var entityTemplateTypeInfos = new List<TypeInfo>();
            var systemTypeInfos = new List<TypeInfo>();

            var componentTypeInfo = typeof(EntityComponent).GetTypeInfo();
            var systemTypeInfo = typeof(EntitySystem).GetTypeInfo();
            var entityTempalteTypeInfo = typeof(EntityTemplate).GetTypeInfo();

            foreach (var assembly in assemblies)
            {
                var typeInfos = assembly.ExportedTypes.Select(x => x.GetTypeInfo()).ToArray();

                foreach (var typeInfo in typeInfos)
                {
                    var isComponent = componentTypeInfo.IsAssignableFrom(typeInfo);
                    if (isComponent)
                    {
                        componentTypeInfos.Add(typeInfo);
                        continue;
                    }

                    var isSystem = systemTypeInfo.IsAssignableFrom(typeInfo);
                    if (isSystem)
                    {
                        systemTypeInfos.Add(typeInfo);
                        continue;
                    }

                    var isEntityTemplate = entityTempalteTypeInfo.IsAssignableFrom(typeInfo);
                    // ReSharper disable once InvertIf
                    if (isEntityTemplate)
                    {
                        entityTemplateTypeInfos.Add(typeInfo);
                        // ReSharper disable once RedundantJumpStatement
                        continue;
                    }
                }
            }

            var registeredComponentCount = RegisterComponents(componentTypeInfos);
            RegisterEntityTemplates(entityTemplateTypeInfos);
            CreateSystems(systemTypeInfos, registeredComponentCount);
        }

        private int RegisterComponents(List<TypeInfo> componentTypeInfos)
        {
            List<TypeInfo> registeredComponentTypeInfos;
            List<Tuple<TypeInfo, EntityComponentPoolAttribute>> registeredPooledComponentTypeInfos;
            GetRegisteredComponents(componentTypeInfos, out registeredComponentTypeInfos, out registeredPooledComponentTypeInfos);

            EntityManager.CreateComponentTypesFrom(registeredComponentTypeInfos);
            EntityManager.CreateComponentPoolsFrom(registeredPooledComponentTypeInfos);
            return registeredComponentTypeInfos.Count;
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static void GetRegisteredComponents(List<TypeInfo> componentTypeInfos, out List<TypeInfo> registeredComponentTypeInfos, out List<Tuple<TypeInfo, EntityComponentPoolAttribute>> pooledComponentTypeInfos)
        {
            registeredComponentTypeInfos = new List<TypeInfo>();
            pooledComponentTypeInfos = new List<Tuple<TypeInfo, EntityComponentPoolAttribute>>();

            var componentAttributeType = typeof(EntityComponentAttribute);
            var componentPoolAttributeType = typeof(EntityComponentPoolAttribute);

            foreach (var typeInfo in componentTypeInfos)
            {
                EntityComponentAttribute componentAttribute = null;
                EntityComponentPoolAttribute componentPoolAttribute = null;

                var attributes = typeInfo.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    var attributeType = attribute.GetType();

                    if (attributeType == componentAttributeType)
                        componentAttribute = (EntityComponentAttribute)attribute;

                    if (attributeType == componentPoolAttributeType)
                        componentPoolAttribute = (EntityComponentPoolAttribute)attribute;
                }

                if (componentAttribute == null)
                    continue;

                registeredComponentTypeInfos.Add(typeInfo);

                if (componentPoolAttribute == null)
                    continue;

                pooledComponentTypeInfos.Add(new Tuple<TypeInfo, EntityComponentPoolAttribute>(typeInfo, componentPoolAttribute));
            }
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private void RegisterEntityTemplates(List<TypeInfo> entityTempalteTypeInfos)
        {
            var entityTemplateAttributeType = typeof(EntityTemplateAttribute);

            foreach (var typeInfo in entityTempalteTypeInfos)
            {

                EntityTemplateAttribute entityTemplateAttribute = null;

                var attributes = typeInfo.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    var attributeType = attribute.GetType();

                    if (attributeType == entityTemplateAttributeType)
                        entityTemplateAttribute = (EntityTemplateAttribute)attribute;
                }

                if (entityTemplateAttribute == null)
                    return;

                var entityTemplate = (EntityTemplate)Activator.CreateInstance(typeInfo.AsType());
                EntityManager.AddEntityTemplate(entityTemplateAttribute.Name, entityTemplate);
            }
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private void CreateSystems(List<TypeInfo> systemTypeInfos, int componentCount)
        {
            var systemAttributeType = typeof(EntitySystemAttribute);
            var aspectAttributeType = typeof(AspectAttribute);

            var andMask = new BitVector(componentCount);
            var orMask = new BitVector(componentCount);
            var norMask = new BitVector(componentCount);

            foreach (var typeInfo in systemTypeInfos)
            {
                EntitySystemAttribute systemAttribute = null;

                andMask.SetAll(false);
                orMask.SetAll(false);
                norMask.SetAll(false);

                var attributes = typeInfo.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    var attributeType = attribute.GetType();

                    if (attributeType == systemAttributeType)
                        systemAttribute = (EntitySystemAttribute)attribute;

                    if(attributeType != aspectAttributeType)
                        continue;

                    var aspectAttribute = (AspectAttribute)attribute;
                    switch (aspectAttribute.Type)
                    {
                        case AspectType.All:
                            EntityManager.FillComponentBits(andMask, aspectAttribute.Components);
                            break;
                        case AspectType.Any:
                            EntityManager.FillComponentBits(orMask, aspectAttribute.Components);
                            break;
                        case AspectType.None:
                            EntityManager.FillComponentBits(norMask, aspectAttribute.Components);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                }

                if (systemAttribute == null)
                    return;

                var system = (EntitySystem)Activator.CreateInstance(typeInfo.AsType());
                var processingSystem = system as EntityProcessingSystem;
                if (processingSystem != null)
                    processingSystem.Aspect = new Aspect(andMask, orMask, norMask);

                _systemManager.AddSystem(system, systemAttribute.GameLoopType,
                    systemAttribute.Layer, SystemExecutionType.Synchronous);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            _systemManager.InitializeIfNecessary();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            foreach (var system in _systemManager.Systems)
                system.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            _systemManager.InitializeIfNecessary();

            EntityManager.RemoveMarkedComponents();
            EntityManager.ProcessMarkedEntitiesWith(_systemManager.ProcessingSystems);

            _systemManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _systemManager.Draw(gameTime);
        }
    }
}