using System;

namespace PubComp.Caching.SystemRuntime
{
    public class ScopedCacheItem : CacheItem
    {
        public Object Value { get; set; }
        public DateTimeOffset ValueTimestamp { get; set; }

        public ScopedCacheItem()
        {
        }

        public ScopedCacheItem(object value)
        {
            this.Value = value;
            this.ValueTimestamp = DateTimeOffset.UtcNow;
        }

        public ScopedCacheItem(Object value, DateTimeOffset valueTimestamp)
        {
            this.Value = value;
            this.ValueTimestamp = valueTimestamp;
        }
    }
}
