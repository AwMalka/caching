﻿namespace PubComp.Caching.Core
{
    public class LayeredCachePolicy
    {
        /// <summary>
        /// Required parameter - level1 cache name
        /// </summary>
        public string Level1CacheName { get; set; }

        /// <summary>
        /// Required parameter - level2 cache name
        /// </summary>
        public string Level2CacheName { get; set; }

        /// <summary>
        /// Use SyncProvider to notify level1 when using updating level2
        /// </summary>
        public bool InvalidateLevel1OnLevel2Update { get; set; }

        /// <summary>
        /// Notifications provider name. Default is null/undefined.
        /// </summary>
        public string SyncProvider { get; set; }

        public LayeredCachePolicy()
        {
            SyncProvider = null;
        }
    }
}
