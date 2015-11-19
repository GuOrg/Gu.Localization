namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal static partial class Ensure
    {
        internal static void LessThan<T>(T value, T max, string parameterName) where T : IComparable<T>
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (Comparer<T>.Default.Compare(value, max) >= 0)
            {
                string message = $"Expected {parameterName} to be less than {max}, {parameterName} was {value}";
                throw new ArgumentException(message, parameterName);
            }
        }

        internal static void LessThanOrEqual<T>(T value, T max, string parameterName) where T : IComparable<T>
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (Comparer<T>.Default.Compare(value, max) > 0)
            {
                string message = $"Expected {parameterName} to be less than or equal to {max}, {parameterName} was {value}";
                throw new ArgumentException(message, parameterName);
            }
        }

        internal static void GreaterThan<T>(T value, T min, string parameterName) where T : IComparable<T>
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (Comparer<T>.Default.Compare(value, min) <= 0)
            {
                string message = $"Expected {parameterName} to be greater than {min}, {parameterName} was {value}";
                throw new ArgumentException(message, parameterName);
            }
        }

        internal static void GreaterThanOrEqual<T>(T value, T min, string parameterName) where T : IComparable<T>
        {
            Debug.Assert(!string.IsNullOrEmpty(parameterName), $"{nameof(parameterName)} cannot be null");
            if (Comparer<T>.Default.Compare(value, min) < 0)
            {
                string message = $"Expected {parameterName} to be greater than or equal to {min}, {parameterName} was {value}";
                throw new ArgumentException(message, parameterName);
            }
        }
    }
}
