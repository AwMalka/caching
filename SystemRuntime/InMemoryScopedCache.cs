using PubComp.Caching.Core;
using PubComp.Caching.Core.Directives;
using System;

namespace PubComp.Caching.SystemRuntime
{
    public class InMemoryScopedCache : ObjectScopedCache
    {
        public InMemoryScopedCache(String name, InMemoryPolicy policy, IScopedContext<CacheDirectives> cacheDirectivesScopedContext)
            : base(name, new System.Runtime.Caching.MemoryCache(name), policy, cacheDirectivesScopedContext)
        {
        }

        public InMemoryScopedCache(String name, TimeSpan slidingExpiration, IScopedContext<CacheDirectives> cacheDirectivesScopedContext)
            : this(name,
                new InMemoryPolicy
                {
                    SlidingExpiration = slidingExpiration
                }, 
                cacheDirectivesScopedContext)
        {
        }

        public InMemoryScopedCache(String name, DateTimeOffset absoluteExpiration, IScopedContext<CacheDirectives> cacheDirectivesScopedContext)
            : this(name,
                new InMemoryPolicy
                {
                    AbsoluteExpiration = absoluteExpiration
                }, 
                cacheDirectivesScopedContext)
        {
        }
    }
}
