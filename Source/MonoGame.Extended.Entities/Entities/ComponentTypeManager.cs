// Original code dervied from:
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/Entities/ComponentTypeManager.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComponentTypeManager.cs" company="GAMADU.COM">
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
//   Class ComponentTypeManager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace MonoGame.Extended.Entities
{
    internal static class ComponentTypeManager
    {
        private static readonly Dictionary<Type, ComponentType> ComponentTypes = new Dictionary<Type, ComponentType>();

        public static BigInteger GetBit<TComponent>() where TComponent : Component
        {
            return GetTypeFor<TComponent>().Bit;
        }

        public static int GetIdentifier<TComponent>() where TComponent : Component
        {
            return GetTypeFor<TComponent>().Index;
        }

        public static ComponentType GetTypeFor<T>() where T : Component
        {
            return GetTypeFor(typeof(T));
        }

        public static ComponentType GetTypeFor(Type type)
        {
            ComponentType result;
            if (ComponentTypes.TryGetValue(type, out result))
                return result;
            result = new ComponentType(type);
            ComponentTypes.Add(type, result);

            return result;
        }

        public static void Initialize(params Assembly[] assembliesToScan)
        {
            foreach (var assembly in assembliesToScan)
            {
                var componentTypeInfo = typeof(Component).GetTypeInfo();

                foreach (var type in assembly.ExportedTypes)
                {
                    var typeInfo = type.GetTypeInfo();

                    if (!componentTypeInfo.IsAssignableFrom(typeInfo))
                        continue;

                    if (type == typeof(Component))
                        continue;

                    GetTypeFor(type);
                }
            }
        }

        internal static IEnumerable<Type> GetTypesFromBits(BigInteger bits)
        {
            return ComponentTypes.Where(keyValuePair => (keyValuePair.Value.Bit & bits) != 0)
                .Select(keyValuePair => keyValuePair.Key);
        }

        internal static void SetTypeFor<T>(ComponentType type)
        {
            ComponentTypes.Add(typeof(T), type);
        }
    }
}