﻿using System;

namespace PubComp.Caching.RedisCaching
{
    public class CacheItem<TValue>
    {
        private const string CacheNamePrefix = "c=";
        private const string KeyPrefix = ":k=";

        public String Id { get; set; }

        public TValue Value { get; set; }

        public DateTimeOffset ValueTimestamp { get; set; }

        public TimeSpan? ExpireIn { get; set; }

        public CacheItem()
        {
        }

        public static String GetId(String cacheName, String key)
        {
            return string.Concat(CacheNamePrefix, cacheName, KeyPrefix, key);
        }

        public CacheItem(String cacheName, String key, TValue value)
        {
            this.Id = GetId(cacheName, key);
            this.Value = value;
            this.ValueTimestamp = DateTimeOffset.UtcNow;
        }

        public CacheItem(String cacheName, String key, TValue value, DateTimeOffset valueTimestamp)
        {
            this.Id = GetId(cacheName, key);
            this.Value = value;
            this.ValueTimestamp = valueTimestamp;
        }

        public CacheItem(String cacheName, String key, TValue value, TimeSpan? expireIn)
        {
            this.Id = GetId(cacheName, key);
            this.Value = value;
            this.ValueTimestamp = DateTimeOffset.UtcNow;
            this.ExpireIn = expireIn;
        }

        public CacheItem(String cacheName, String key, TValue value, TimeSpan? expireIn, DateTimeOffset valueTimestamp)
        {
            this.Id = GetId(cacheName, key);
            this.Value = value;
            this.ExpireIn = expireIn;
            this.ValueTimestamp = valueTimestamp;
        }
    }
}
