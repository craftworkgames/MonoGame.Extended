// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using MonoGame.Extended.Particles.Data;
using TPL = System.Threading.Tasks.Parallel;

namespace MonoGame.Extended.Particles.Modifiers;

/// <summary>
/// Defines different strategies for executing particle modifiers within a particle system.
/// </summary>
/// <remarks>
/// The strategy pattern implemented by this class allows the particle system to switch
/// between serial (single-threaded) and parallel (multi-threaded) execution methods
/// to optimize performance based on the execution environment and particle workload.
/// </remarks>
public abstract class ModifierExecutionStrategy
{
    /// <summary>
    /// Gets a singleton instance of the serial execution strategy.
    /// </summary>
    /// <remarks>
    /// The serial strategy processes each modifier one after another on a single thread.
    /// This may be more efficient for small particle counts or when thread synchronization
    /// overhead outweighs the benefits of parallelism.
    /// </remarks>
    static public ModifierExecutionStrategy Serial = new SerialModifierExecutionStrategy();

    /// <summary>
    /// Gets a singleton instance of the parallel execution strategy.
    /// </summary>
    /// <remarks>
    /// The parallel strategy processes modifiers concurrently using multiple threads.
    /// This can significantly improve performance for systems with many particles and
    /// multiple modifiers, especially on multi-core processors.
    /// </remarks>
    static public ModifierExecutionStrategy Parallel = new ParallelModifierExecutionStrategy();

    /// <summary>
    /// Executes all modifiers in the collection on the particle buffer using the implemented strategy.
    /// </summary>
    /// <param name="modifiers">The collection of modifiers to execute.</param>
    /// <param name="elapsedSeconds">The elapsed time, in seconds, since the last update.</param>
    /// <param name="particle">A pointer to the beginning of the particle buffer.</param>
    /// <param name="count">The total number of particles in the buffer.</param>
    internal abstract unsafe void ExecuteModifiers(IEnumerable<Modifier> modifiers, float elapsedSeconds, Particle* particle, int count);

    /// <summary>
    /// Implements a serial (single-threaded) execution strategy for particle modifiers.
    /// </summary>
    /// <remarks>
    /// This strategy processes each modifier sequentially on a single thread,
    /// which can be more efficient for smaller particle counts or when the
    /// overhead of thread synchronization would outweigh the benefits of parallelism.
    /// </remarks>
    internal class SerialModifierExecutionStrategy : ModifierExecutionStrategy
    {
        internal override unsafe void ExecuteModifiers(IEnumerable<Modifier> modifiers, float elapsedSeconds, Particle* particle, int count)
        {
            foreach (var modifier in modifiers)
            {
                modifier.InternalUpdate(elapsedSeconds, particle, count);
            }
        }

        public override string ToString()
        {
            return nameof(Serial);
        }
    }

    /// <summary>
    /// Implements a parallel (multi-threaded) execution strategy for particle modifiers.
    /// </summary>
    /// <remarks>
    /// This strategy processes modifiers concurrently. It can significantly improve
    /// performance for systems with many particles and multiple modifiers.
    /// </remarks>
    internal class ParallelModifierExecutionStrategy : ModifierExecutionStrategy
    {
        internal override unsafe void ExecuteModifiers(IEnumerable<Modifier> modifiers, float elapsedSeconds, Particle* particle, int count)
        {
            TPL.ForEach(modifiers, modifier => modifier.InternalUpdate(elapsedSeconds, particle, count));
        }

        public override string ToString()
        {
            return nameof(Parallel);
        }
    }
}
