// Original code dervied from:
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/Aspect.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Aspect.cs" company="GAMADU.COM">
//     Copyright © 2013 GAMADU.COM. AllOf rights reserved.
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
//   Specify a Filter class to filter what Entities (with what Components) a EntitySystem will ProcessInternal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq.Expressions;

namespace MonoGame.Extended.Entities
{
    internal class Aspect
    {
        // match entities with components using boolean logic: 
        // (A and B and C and ..) AND (D or E or F or ...) AND NOT(G or H or I or ..)

        // match components that have: 
        //  all of the set {A, B, C, ..} and 
        //  have any of the set {D, E, F, ..} but
        //  do not have any of the set {G, H, I, ..}
        // all(A, B, C, ..).any(D, E, F, ..).no(G, H, I, ..)

        // e.g does match
        // components:  010001110
        // andMask:     000001110
        // orMask:      111000000
        // norMask:     000000001

        // e.g does match
        // components:  100011110
        // andMask:     000001110
        // orMask:      111000000
        // norMask:     000000001

        // e.g does not match
        // components:  100001111
        // andMask:     000011110
        // orMask:      111000000
        // norMask:     000000001

        // e.g does not match
        // components:  000001111
        // andMask:     000011110
        // orMask:      111000000
        // norMask:     000100000

        internal BitVector AndMask;
        internal BitVector OrMask;
        internal BitVector NorMask;
        internal static BitVector Result;

        public Aspect(BitVector andMask, BitVector orMask, BitVector norMask)
        {
            if (Result == null)
                Result = new BitVector(andMask.Length);
            AndMask = new BitVector(andMask);
            OrMask = orMask.EqualsZero() ? new BitVector(andMask.Length, true) : new BitVector(orMask);
            NorMask = new BitVector(norMask);
        }

        internal bool Matches(BitVector componentBits)
        {
            AndMask.And(componentBits, ref Result);
            if (!Result.Equals(AndMask))
                return false;
            NorMask.And(componentBits, ref Result);
            if (!Result.EqualsZero())
                return false;
            OrMask.And(componentBits, ref Result);
            return !Result.EqualsZero();
        }
    }
}