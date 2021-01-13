using System;
using System.Collections.Generic;
using System.Text;

namespace EduLocate.Common
{
    /// <summary>Helper class for working with doubles.</summary>
    public static class DoubleHelper
    {
        /// <summary>Helper to check that two double?s are comparably-similar in value.</summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="absoluteAcceptableDifference">An absolute value of how different they are allowed to be.</param>
        /// <returns></returns>
        public static bool ValuesClose(double? value1, double? value2, double absoluteAcceptableDifference)
        {
            // Return true if both null
            if (value1 == null && value2 == null) return true;

            // Return false if only one is null
            if (value1 == null || value2 == null) return false;

            // Otherwise check that their values are similar
            return Math.Abs(value1.Value - value2.Value) <= absoluteAcceptableDifference;
        }
    }
}
