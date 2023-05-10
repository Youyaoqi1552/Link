using System.Collections.Generic;

namespace Common.Utility
{
    public class DataBundle
    {
        private Dictionary<string, object> dataSource;
        
        public void Set<T>(string key, T value)
        {
            dataSource ??= new Dictionary<string, object>();
            dataSource[key] = value;
        }
        
        public T Get<T>(string key, T defaultValue = default)
        {
            if (dataSource == null)
            {
                return defaultValue;
            }
            
            if (dataSource.TryGetValue(key, out var value))
            {
                return (T) value;
            }
            return defaultValue;
        }
    }
}