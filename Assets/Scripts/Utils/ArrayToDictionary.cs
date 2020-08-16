using System.Collections.Generic;

namespace Utils
{
    public interface IDictionaryEntity<T, TS>
    {
        T TValue { get; }
        TS TsValue { get; }
    }

    public class ArrayToDictionary
    {
        public static Dictionary<T, TS> ToDictionary<T, TS, TSs>(TSs[] entities) where TSs : IDictionaryEntity<T, TS>
        {
            var dictionary = new Dictionary<T, TS>();
            for (var i = 0; i < entities.Length; i++)
            {
                dictionary.Add(entities[i].TValue, entities[i].TsValue);
            }

            return dictionary;
        }
    }
}