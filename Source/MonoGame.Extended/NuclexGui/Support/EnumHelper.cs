using System;

namespace MonoGame.Extended.NuclexGui.Support
{
    /// <summary>Helper methods for enumerations</summary>
    public static class EnumHelper
    {
        /// <summary>Returns the highest value encountered in an enumeration</summary>
        /// <typeparam name="T">
        ///     Enumeration of which the highest value will be returned
        /// </typeparam>
        /// <returns>The highest value in the enumeration</returns>
        public static T GetHighestValue<T>() where T : IComparable
        {
            var values = GetValues<T>();

            // If the enumeration is empty, return nothing
            if (values.Length == 0)
                return default(T);

            // Look for the highest value in the enumeration. We initialize the highest value
            // to the first enumeration value so we don't have to use some arbitrary starting
            // value which might actually appear in the enumeration.
            var highestValue = values[0];
            for (var index = 1; index < values.Length; ++index)
                if (values[index].CompareTo(highestValue) > 0)
                    highestValue = values[index];

            return highestValue;
        }

        /// <summary>Returns the lowest value encountered in an enumeration</summary>
        /// <typeparam name="T">
        ///     Enumeration of which the lowest value will be returned
        /// </typeparam>
        /// <returns>The lowest value in the enumeration</returns>
        public static T GetLowestValue<T>() where T : IComparable
        {
            var values = GetValues<T>();

            // If the enumeration is empty, return nothing
            if (values.Length == 0)
                return default(T);

            // Look for the lowest value in the enumeration. We initialize the lowest value
            // to the first enumeration value so we don't have to use some arbitrary starting
            // value which might actually appear in the enumeration.
            var lowestValue = values[0];
            for (var index = 1; index < values.Length; ++index)
                if (values[index].CompareTo(lowestValue) < 0)
                    lowestValue = values[index];

            return lowestValue;
        }

        /// <summary>Retrieves a list of all values contained in an enumeration</summary>
        /// <typeparam name="T">
        ///     Type of the enumeration whose values will be returned
        /// </typeparam>
        /// <returns>All values contained in the specified enumeration</returns>
        /// <remarks>
        ///     This method produces collectable garbage so it's best to only call it once
        ///     and cache the result.
        /// </remarks>
        public static T[] GetValues<T>()
        {
            return (T[]) Enum.GetValues(typeof(T));
        }
    }
}