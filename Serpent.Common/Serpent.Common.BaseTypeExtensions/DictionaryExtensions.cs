using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serpent.Common.BaseTypeExtensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, 
            TValue @default = default(TValue))
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : @default;
        }

    }
}
