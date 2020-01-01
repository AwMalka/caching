using System;

namespace PubComp.Caching.SystemRuntime
{
    public class CacheItem
    {
        public Object Value { get; set; }

        public CacheItem()
        {
        }

        public CacheItem(object value)
        {
            this.Value = value;
        }
    }
}
