using PubComp.Caching.Core.Directives;
using System;
using System.Threading.Tasks;

namespace PubComp.Caching.Core
{
    public interface IScopedCache : ICache
    {
        IScopedContext<CacheDirectives> CacheDirectivesScopedContext { get; }

        bool TryGet<TValue>(String key, out TValue value, DateTimeOffset minimumContentTimestamp);

        Task<TryGetResult<TValue>> TryGetAsync<TValue>(String key, DateTimeOffset minimumContentTimestamp);

        void Set<TValue>(String key, TValue value, DateTimeOffset contentTimestamp);

        Task SetAsync<TValue>(String key, TValue value, DateTimeOffset contentTimestamp);

        TValue Get<TValue>(String key, Func<TValue> getter, DateTimeOffset minimumContentTimestamp);

        Task<TValue> GetAsync<TValue>(String key, Func<Task<TValue>> getter, DateTimeOffset minimumContentTimestamp);
    }
}
