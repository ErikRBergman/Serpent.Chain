using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Serpent.Common.DapperExtensions
{
    public class DapperParameters : DynamicParameters, IDictionary<string, object>
    {
        private readonly Dictionary<string, Parameter> internalDictionary = new Dictionary<string, Parameter>();

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.internalDictionary.Select(p =>  new KeyValuePair<string, object>(p.Key, p.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            if (this.internalDictionary.TryGetValue(item.Key, out var parameter))
            {
                return parameter.Value == item.Value;
            }

            return false;
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public int Count => this.internalDictionary.Count;

        public bool IsReadOnly => false;

        public bool ContainsKey(string key)
        {
            return this.internalDictionary.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            this.Add(key, value, null, null, null, null, null);
        }

        public new void Add(string name, object value = null, DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null, byte? scale = null)
        {
            this.internalDictionary.Add(name, new Parameter(name, value, dbType, direction, size, precision, scale));
            base.Add(name, value, dbType, direction, size, precision, scale);
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out object value)
        {
            if (this.internalDictionary.TryGetValue(key, out var parameter))
            {
                value = parameter.Value;
                return true;
            }

            value = null;
            return false;
        }

        public object this[string key]
        {
            get
            {
                if (this.internalDictionary.TryGetValue(key, out var parameter))
                {
                    return parameter.Value;
                }

                throw new KeyNotFoundException();
            }

            set => this.Add(key, value);
        }

        public ICollection<string> Keys => this.internalDictionary.Keys;
        public ICollection<object> Values => this.internalDictionary.Values.Select(p => p.Value).ToArray();
    }
}
