using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GoogleCanlendarService.Utils
{
    internal class CommonUtils
    {

        public static void WriteLog(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        public static bool AreListsEqual<T>(IEnumerable<T> list1, IEnumerable<T> list2, params string[] ignoreProperties)
        {
            if (list1 == null && list2 == null)
                return true;

            if ((list1 == null && list2 != null) || list1 != null && list2 == null)
                return false;

            // Check if the lists have the same count
            if (list1.Count() != list2.Count())
                return false;

            // Sort both lists for consistent comparison
            var sortedList1 = list1.OrderBy(e => e.GetHashCode()).ToList();
            var sortedList2 = list2.OrderBy(e => e.GetHashCode()).ToList();

            // Iterate through both lists and compare each object
            for (int i = 0; i < sortedList1.Count; i++)
            {
                if (!AreObjectsEqual(sortedList1[i], sortedList2[i], ignoreProperties))
                {
                    return false; // If any object pair doesn't match, the lists are not equal
                }
            }

            return true; // All objects matched
        }

        // Helper function to compare two objects, ignoring certain properties
        private static bool AreObjectsEqual<T>(T obj1, T obj2, params string[] ignoreProperties)
        {
            // Get the public properties of the object type
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !ignoreProperties.Contains(p.Name)); // Exclude ignored properties

            foreach (var property in properties)
            {
                var value1 = property.GetValue(obj1);
                var value2 = property.GetValue(obj2);

                // Compare the property values; handle nulls appropriately
                if (value1 == null && value2 == null)
                    continue;

                if (value1 == null || value2 == null || !value1.Equals(value2))
                {
                    return false; // Property values do not match
                }
            }

            return true; // All property values match
        }
    }
}
