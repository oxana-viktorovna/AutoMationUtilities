using System.Collections.Generic;

namespace SharedCore.BuildInTypesExtentions
{
    public static class DictionaryExtension
    {
        public static Dictionary<TKey, TValue> ConcatenateWith<TKey, TValue>(this Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            Dictionary<TKey, TValue> result = new (dict1);

            foreach (var kvp in dict2)
            {
                // Add the element to the dictionary, or update the value if the key already exists
                result[kvp.Key] = kvp.Value;
            }

            return result;
        }

        public static void ConcatenateWith2<TKey, TValue>(this Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            foreach (var kvp in dict2)
            {
                // Add the element to the dictionary, or update the value if the key already exists
                dict1[kvp.Key] = kvp.Value;
            }
        }
    }
}
