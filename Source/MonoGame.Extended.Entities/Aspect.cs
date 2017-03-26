// Original code dervied from:
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/Aspect.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Aspect.cs" company="GAMADU.COM">
//     Copyright © 2013 GAMADU.COM. RequiresAllOf rights reserved.
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
//   Specify a Filter class to filter what Entities (with what Components) a EntitySystem will Process.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MonoGame.Extended.Entities
{
    public class Aspect
    {
        protected BigInteger ContainsTypesMap { get; set; }
        protected BigInteger ExcludeTypesMap { get; set; }
        protected BigInteger OneTypesMap { get; set; }

        protected Aspect()
        {
            OneTypesMap = 0;
            ExcludeTypesMap = 0;
            ContainsTypesMap = 0;
        }

        internal static Aspect AllOf(params Type[] types)
        {
            return new Aspect().GetAll(types);
        }

        internal static Aspect Empty()
        {
            return new Aspect();
        }

        internal static Aspect NoneOf(params Type[] types)
        {
            return new Aspect().GetExclude(types);
        }

        internal static Aspect OneOf(params Type[] types)
        {
            return new Aspect().GetOne(types);
        }

        public virtual bool IsInterestedIn(Entity entity)
        {
            if (entity == null)
                return false;

            if (!(ContainsTypesMap > 0 || ExcludeTypesMap > 0 || OneTypesMap > 0))
                return false;

            ////Little help
            ////10010 & 10000 = 10000
            ////10010 | 10000 = 10010
            ////10010 | 01000 = 11010

            ////1001 & 0000 = 0000 OK
            ////1001 & 0100 = 0000 NOK           
            ////0011 & 1001 = 0001 Ok

            return ((OneTypesMap & entity.TypeBits) != 0 || OneTypesMap == 0) &&
                   ((ContainsTypesMap & entity.TypeBits) == ContainsTypesMap || ContainsTypesMap == 0) &&
                   ((ExcludeTypesMap & entity.TypeBits) == 0 || ExcludeTypesMap == 0);
        }

        internal Aspect GetAll(params Type[] types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            foreach (var componentType in types.Select(ComponentTypeManager.GetTypeFor))
                ContainsTypesMap |= componentType.Bit;

            return this;
        }

        internal Aspect GetExclude(params Type[] types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            foreach (var componentType in types.Select(ComponentTypeManager.GetTypeFor))
                ExcludeTypesMap |= componentType.Bit;

            return this;
        }

        internal Aspect GetOne(params Type[] types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            foreach (var componentType in types.Select(ComponentTypeManager.GetTypeFor))
                OneTypesMap |= componentType.Bit;

            return this;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(1024);

            builder.AppendLine("Aspect :");
            AppendTypesAsString(builder, " RequiresAllOf the components : ", ContainsTypesMap);
            AppendTypesAsString(builder, " RequiresAllOf none of the components : ", ExcludeTypesMap);
            AppendTypesAsString(builder, " RequiresAllOf atleast one of the components : ", OneTypesMap);

            return builder.ToString();
        }

        private static void AppendTypesAsString(StringBuilder builder, string headerMessage, BigInteger typeBits)
        {
            if (typeBits == 0)
                return;

            builder.AppendLine(headerMessage);
            foreach (var type in ComponentTypeManager.GetTypesFromBits(typeBits))
            {
                builder.Append(", ");
                builder.AppendLine(type.Name);
            }
        }
    }
}