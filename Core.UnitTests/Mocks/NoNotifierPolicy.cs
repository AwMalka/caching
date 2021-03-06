﻿namespace PubComp.Caching.Core.UnitTests.Mocks
{
    public class NoNotifierPolicy
    {
        /// <summary>
        /// Connection string to Redis. You must either fill this in or ConnectionName.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Connection string name. You must either fill this in or ConnectionString.
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        /// Optional - Automatic publish CacheItemActionTypes.Updated when overriding cache item with new value
        /// </summary>
        public bool InvalidateOnUpdate { get; set; }
    }
}
