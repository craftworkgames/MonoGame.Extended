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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities
{
    public delegate void EntityDelegate(Entity entity);
    public delegate void EntityComponentDelegate(Entity entity, Component component);

    public sealed partial class EntityComponentSystemManager : DrawableGameComponent
    {
        private bool _hasInitialized;

        public EntityComponentSystemManager(Game game)
            : base(game)
        {
        }

        public void Scan(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.ExportedTypes;
                foreach (var type in types)
                   Scan(type);
            }
        }

        [SuppressMessage("ReSharper", "InvertIf")]
        private void Scan(Type type)
        {
            var systemTypeInfo = typeof(System).GetTypeInfo();
            var systemAttributeType = typeof(SystemAttribute);

            var entityTemplateTypeInfo = typeof(EntityTemplate).GetTypeInfo();
            var entityTemplateAttributeType = typeof(EntityTemplateAttribute);

            var componentTypeInfo = typeof(Component).GetTypeInfo();
            var componentPoolAttributeType = typeof(ComponentPoolAttribute);

            var typeInfo = type.GetTypeInfo();
            var attributes = type.GetTypeInfo().GetCustomAttributes(false);

            var isSystem = systemTypeInfo.IsAssignableFrom(typeInfo);
            var isEntityTemplate = entityTemplateTypeInfo.IsAssignableFrom(typeInfo);
            var isComponent = componentTypeInfo.IsAssignableFrom(typeInfo);

            if (!isSystem && !isEntityTemplate && !isComponent)
                return;

            foreach (var attribute in attributes)
            {
                var attributeType = attribute.GetType();

                if (isSystem && attributeType == systemAttributeType)
                {
                    var system = (System)Activator.CreateInstance(type);
                    var systemAttribute = (SystemAttribute)attribute;

                    Aspect aspect;

                    switch (systemAttribute.AspectType)
                    {
                        case AspectType.RequiresAllOf:
                            aspect = Aspect.AllOf(systemAttribute.ComponentTypes);
                            break;
                        case AspectType.RequiresNoneOf:
                            aspect = Aspect.NoneOf(systemAttribute.ComponentTypes);
                            break;
                        case AspectType.RequiresAtleastOneOf:
                            aspect = Aspect.OneOf(systemAttribute.ComponentTypes);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    system.Aspect = aspect;

                    AddSystem(system, systemAttribute.GameLoopType, systemAttribute.Layer); //, systemAttribute.ExecutionType);
                    break;
                }

                if (isEntityTemplate && attributeType == entityTemplateAttributeType)
                {
                    var entityTemplate = (EntityTemplate)Activator.CreateInstance(type);
                    var entityTemplateAttribute = (EntityTemplateAttribute)attribute;
                    AddEntityTemplate(entityTemplateAttribute.Name, entityTemplate);
                    break;
                }

                if (isComponent && attributeType == componentPoolAttributeType)
                {
                    var componentPoolAttribute = (ComponentPoolAttribute)attribute;
                    var componentType = ComponentTypeManager.GetTypeFor(type);
                    CreateComponentPool(componentType, componentPoolAttribute.Capacity);
                    break;
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            InitializeIfNecessary();
        }

        private void InitializeIfNecessary()
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

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            foreach (var system in Systems)
                system.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            InitializeIfNecessary();
            RemoveMarkedComponents();
            RemoveMarkedEntities();
            RefreshMarkedEntities();
            UpdateSystems(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawSystems(gameTime);
        }
    }
}