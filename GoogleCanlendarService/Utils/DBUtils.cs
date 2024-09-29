using System;

namespace GoogleCanlendarService.Utils
{
    class DBUtils
    {
        /// <summary>
        /// Returns the original value if it's not null, otherwise, returns DBNull.Value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>The original value or DBNull.Value.</returns>
        public static object GetDBValue(object value)
        {
            return value ?? DBNull.Value;
        }
    }
}
